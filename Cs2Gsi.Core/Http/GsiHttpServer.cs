using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Cs2Gsi.Core.Config;
using Cs2Gsi.Core.Models;
using Cs2Gsi.Core.Parsing;

namespace Cs2Gsi.Core.Http;

public sealed class GsiHttpServer : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    private readonly string token;
    private readonly HttpListener listener = new();
    private readonly object syncRoot = new();

    private GameState lastState = new();
    private JsonObject? cumulativePayload;
    private string lastRawPayload = "{}";
    private DateTime? bombPlantedAt;
    private bool started;

    public GsiHttpServer(string token = GsiDefaults.AuthToken, int port = GsiDefaults.Port)
    {
        this.token = token;
        Prefix = GsiDefaults.Prefix(port);
        listener.Prefixes.Add(Prefix);
    }

    public string Prefix { get; }

    public Action<GameState>? StateReceived { get; set; }

    public Action<Exception>? RequestFailed { get; set; }

    public void Start()
    {
        lock (syncRoot)
        {
            if (started)
            {
                return;
            }

            listener.Start();
            started = true;
            _ = Task.Run(AcceptLoopAsync);
        }
    }

    public GameState GetLastState()
    {
        lock (syncRoot)
        {
            return lastState;
        }
    }

    private async Task AcceptLoopAsync()
    {
        while (listener.IsListening)
        {
            try
            {
                var context = await listener.GetContextAsync().ConfigureAwait(false);
                _ = Task.Run(() => HandleRequestAsync(context));
            }
            catch (Exception ex) when (ex is HttpListenerException or ObjectDisposedException or InvalidOperationException)
            {
                break;
            }
        }
    }

    private async Task HandleRequestAsync(HttpListenerContext context)
    {
        try
        {
            if (context.Request.HttpMethod == "GET" &&
                context.Request.Url?.AbsolutePath == GsiDefaults.StatePath)
            {
                await WriteJsonAsync(context.Response, GetLastState()).ConfigureAwait(false);
                return;
            }

            if (context.Request.HttpMethod == "GET" &&
                context.Request.Url?.AbsolutePath == GsiDefaults.RawPath)
            {
                await WriteStringAsync(context.Response, GetLastRawPayload(), "application/json").ConfigureAwait(false);
                return;
            }

            if (context.Request.HttpMethod == "POST")
            {
                await HandlePostAsync(context).ConfigureAwait(false);
                return;
            }

            context.Response.StatusCode = 404;
        }
        catch (JsonException ex)
        {
            RequestFailed?.Invoke(ex);
            context.Response.StatusCode = 400;
        }
        catch (Exception ex)
        {
            RequestFailed?.Invoke(ex);
            context.Response.StatusCode = 500;
        }
        finally
        {
            context.Response.Close();
        }
    }

    private async Task HandlePostAsync(HttpListenerContext context)
    {
        using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
        var body = await reader.ReadToEndAsync().ConfigureAwait(false);
        var incomingState = GsiPayloadParser.Parse(body);

        if (incomingState.AuthToken != token)
        {
            context.Response.StatusCode = 403;
            return;
        }

        var state = ParseMergedState(body);
        ApplyBombTimer(state);

        lock (syncRoot)
        {
            lastState = state;
            lastRawPayload = body;
        }

        StateReceived?.Invoke(state);
        context.Response.StatusCode = 200;
    }

    private void ApplyBombTimer(GameState state)
    {
        if (string.Equals(state.Bomb.State, "planted", StringComparison.OrdinalIgnoreCase))
        {
            bombPlantedAt ??= DateTime.UtcNow;

            if (string.IsNullOrWhiteSpace(state.Bomb.Timer))
            {
                var elapsed = (DateTime.UtcNow - bombPlantedAt.Value).TotalSeconds;
                var remaining = Math.Max(0, 40 - elapsed);
                state.Bomb.Timer = $"{(int)remaining}s";
            }

            state.Bomb.PlantedAt = bombPlantedAt;
            return;
        }

        bombPlantedAt = null;
    }

    private string GetLastRawPayload()
    {
        lock (syncRoot)
        {
            return lastRawPayload;
        }
    }

    private GameState ParseMergedState(string body)
    {
        var payload = JsonNode.Parse(body)?.AsObject()
            ?? throw new JsonException("CS2 GSI payload root was not a JSON object.");

        JsonObject merged;
        lock (syncRoot)
        {
            cumulativePayload = cumulativePayload is null
                ? CloneObject(payload)
                : MergeObjects(cumulativePayload, payload);

            merged = CloneObject(cumulativePayload);
        }

        return GsiPayloadParser.Parse(merged.ToJsonString());
    }

    private static JsonObject MergeObjects(JsonObject target, JsonObject patch)
    {
        if (patch["previously"] is JsonObject previously)
        {
            RemovePreviousOnlyValues(target, previously, patch);
        }

        foreach (var property in patch)
        {
            if (IsDeltaMetadata(property.Key))
            {
                continue;
            }

            if (property.Value is JsonObject patchObject &&
                target[property.Key] is JsonObject targetObject)
            {
                MergeObjects(targetObject, patchObject);
                continue;
            }

            target[property.Key] = property.Value?.DeepClone();
        }

        return target;
    }

    private static void RemovePreviousOnlyValues(JsonObject target, JsonObject previously, JsonObject patch)
    {
        foreach (var property in previously)
        {
            if (IsDeltaMetadata(property.Key))
            {
                continue;
            }

            if (!patch.TryGetPropertyValue(property.Key, out var patchValue))
            {
                target.Remove(property.Key);
                continue;
            }

            if (property.Value is JsonObject previousObject &&
                patchValue is JsonObject patchObject &&
                target[property.Key] is JsonObject targetObject)
            {
                RemovePreviousOnlyValues(targetObject, previousObject, patchObject);
            }
        }
    }

    private static bool IsDeltaMetadata(string key)
    {
        return key is "previously" or "added";
    }

    private static JsonObject CloneObject(JsonObject source)
    {
        return source.DeepClone().AsObject();
    }

    private static async Task WriteJsonAsync(HttpListenerResponse response, GameState state)
    {
        var json = JsonSerializer.Serialize(state, JsonOptions);
        await WriteStringAsync(response, json, "application/json").ConfigureAwait(false);
    }

    private static async Task WriteStringAsync(HttpListenerResponse response, string value, string contentType)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        response.ContentType = contentType;
        response.ContentEncoding = Encoding.UTF8;
        response.ContentLength64 = bytes.Length;
        await response.OutputStream.WriteAsync(bytes).ConfigureAwait(false);
    }

    public void Dispose()
    {
        lock (syncRoot)
        {
            if (!started)
            {
                return;
            }

            listener.Close();
            started = false;
        }
    }
}
