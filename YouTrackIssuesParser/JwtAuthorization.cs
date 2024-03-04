using System.Net.Http.Headers;

namespace YouTrackIssuesParser;

public class JwtAuthorization : IHttpAuthorization
{
    private readonly string _token;

    public JwtAuthorization(string token)
    {
        _token = token;
    }
    
    public AuthenticationHeaderValue Authorize()
    {
        return new AuthenticationHeaderValue("Bearer", _token);
    }
}