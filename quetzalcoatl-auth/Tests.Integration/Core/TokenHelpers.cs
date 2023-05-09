namespace Tests.Integration.Core;

public static class TokenHelpers
{
    public static string ExtractTokenFromResponse(HttpResponseMessage response)
    {
        var token = response.Headers.GetValues("Set-Cookie").First();
        token = token.Split(';')[0];
        token = token.Split('=')[1];
        return token;
    }
}