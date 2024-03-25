using System.Net.Http.Headers;

namespace YouTrackIssuesParser.Services;

/// <summary>
/// Сервис аутентификации http запроса с помощью jwt
/// </summary>
public class JwtAuthenticationService : IHttpAuthenticationService
{
    /// <summary>
    /// Json web token
    /// </summary>
    private readonly string _token;

    public JwtAuthenticationService(string token)
    {
        _token = token;
    }
    
    /// <inheritdoc cref="IHttpAuthenticationService.Authenticate"/>
    public AuthenticationHeaderValue Authenticate()
    {
        return new AuthenticationHeaderValue("Bearer", _token);
    }
}