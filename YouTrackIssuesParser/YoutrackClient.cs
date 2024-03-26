using System.Net.Http.Json;
using YouTrackIssuesParser.Models;
using YouTrackIssuesParser.Services;

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
    /// Загрузка набора задач с YouTrack
    /// </summary>
    /// <param name="query">Параметры запроса</param>
    /// <returns>Десериализованный лист с задачами</returns>
    public async Task<List<Issue>> LoadIssues(string query = "fields=id")
    {
        return await MakeRequest<List<Issue>>($"{_uri}?{query}");
    }

    /// <summary>
    /// Загрузка WorkLog`ов задачи
    /// </summary>
    /// <param name="id">Идентификатор задачи</param>
    /// <param name="query">Параметры запроса</param>
    /// <returns>Десериализованный объект TimeTracking</returns>
    public async Task<TimeTracking> LoadTimeTracking(string id, string query = "fields=id")
    {
        return await MakeRequest<TimeTracking>($"{_uri}/{id}/timeTracking?{query}");
    }

    /// <summary>
    /// Отправление запроса
    /// </summary>
    /// <param name="uri">Uri для отправления запроса</param>
    /// <returns>Десериализованный объект</returns>
    private async Task<T> MakeRequest<T>(string uri) where T: new()
    {
        using (var message = new HttpRequestMessage(HttpMethod.Get, uri))
        {
            message.Headers.Authorization = _httpAuthenticationService.Authenticate();
            var response = await _httpClient.SendAsync(message);
            response.EnsureSuccessStatusCode();
            var result =  await response.Content.ReadFromJsonAsync<T>();

            if (result == null) throw new ArgumentNullException(nameof(T));

            return result;
        }
    }
}