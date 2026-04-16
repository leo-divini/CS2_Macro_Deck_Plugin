namespace Cs2Gsi.Core.Config;

public static class GsiDefaults
{
    public const string AuthToken = "cs2md_token_segreto";
    public const int Port = 3333;
    public const string StatePath = "/state";

    public static string Prefix(int port = Port) => $"http://127.0.0.1:{port}/";
}
