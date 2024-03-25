using System.Net.Http.Headers;

namespace YouTrackIssuesParser;

/// <summary>
/// Сервис аутентификации http запроса
/// </summary>
public interface IHttpAuthenticationService
{
    /// <summary>
    /// Получение http заголовка аутентификации
    /// </summary>
    /// <returns>Http заголовок аутентификации</returns>
    AuthenticationHeaderValue Authenticate();
}