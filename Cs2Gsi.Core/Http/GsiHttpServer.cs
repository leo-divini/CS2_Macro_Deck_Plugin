using System.Net;
using System.Text;
using System.Text.Json;
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
        var state = GsiPayloadParser.Parse(body);

        if (state.AuthToken != token)
        {
            context.Response.StatusCode = 403;
            return;
        }

        ApplyBombTimer(state);

        lock (syncRoot)
        {
            lastState = state;
        }

        StateReceived?.Invoke(state);
        context.Response.StatusCode = 200;
    }

    private void ApplyBombTimer(GameState state)
    {
        if (state.Bomb.State == "planted")
        {
            bombPlantedAt ??= DateTime.UtcNow;
            var elapsed = (DateTime.UtcNow - bombPlantedAt.Value).TotalSeconds;
            var remaining = Math.Max(0, 40 - elapsed);
            state.Bomb.Timer = $"{(int)remaining}s";
            state.Bomb.PlantedAt = bombPlantedAt;
            return;
        }

        bombPlantedAt = null;
        state.Bomb.Timer = "";
    }

    private static async Task WriteJsonAsync(HttpListenerResponse response, GameState state)
    {
        var json = JsonSerializer.Serialize(state, JsonOptions);
        var bytes = Encoding.UTF8.GetBytes(json);
        response.ContentType = "application/json";
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
