namespace YouTrackIssuesParser;

/// <summary>
/// Клиент для работы с YouTrack
/// </summary>
public class YoutrackClient
{
    /// <summary>
    /// Http клиент для отправки и получения запросов
    /// </summary>
    private readonly HttpClient _httpClient;
    
    /// <inheritdoc cref="IHttpAuthenticationService"/>
    private readonly IHttpAuthenticationService _httpAuthenticationService;
    
    /// <summary>
    /// Uri ресурса задач YouTrack
    /// </summary>
    private readonly string _uri;

    public YoutrackClient(HttpClient httpClient, string source, IHttpAuthenticationService httpAuthenticationService)
    {
        _httpClient = httpClient;
        _uri = $"https://{source}.youtrack.cloud/api/issues";
        _httpAuthenticationService = httpAuthenticationService;
    }

    /// <summary>
    /// Загрузка задачи с YouTrack
    /// </summary>
    /// <param name="query">Параметры запроса</param>
    /// <returns>Json с задачами</returns>
    public async Task<string> LoadIssues(string query = "fields=id")
    {
        return await MakeRequest($"{_uri}?{query}");
    }

    /// <summary>
    /// Загрузка WorkLog`ов задачи
    /// </summary>
    /// <param name="id">Идентификатор задачи</param>
    /// <param name="query">Параметры запроса</param>
    /// <returns>Json WorkLog`ов задачи</returns>
    public async Task<string> LoadTimeTracking(string id, string query = "fields=id")
    {
        return await MakeRequest($"{_uri}/{id}/timeTracking?{query}");
    }

    /// <summary>
    /// Отправление запроса
    /// </summary>
    /// <param name="uri">Uri для отправления запроса</param>
    /// <returns>Строковое содержимое http ответа</returns>
    private async Task<string> MakeRequest(string uri)
    {
        using (var message = new HttpRequestMessage(HttpMethod.Get, uri))
        {
            message.Headers.Authorization = _httpAuthenticationService.Authenticate();
            var response = await _httpClient.SendAsync(message);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}