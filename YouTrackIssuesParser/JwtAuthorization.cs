using System.Net.Http.Headers;

namespace YouTrackIssuesParser;

public class JwtAuthorization : IHttpAuthorization
{
    private readonly string token;

    public JwtAuthorization(string token)
    {
        this.token = token;
    }
    
    public void Authorize(HttpRequestMessage message)
    {
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}