namespace YouTrackIssuesParser;

public class IssuesLoader
{
    private readonly HttpClient _httpClient;
    private readonly IHttpAuthorization _httpAuthorization;
    private readonly string _uri;

    public IssuesLoader(HttpClient httpClient, string source, IHttpAuthorization httpAuthorization)
    {
        _httpClient = httpClient;
        _uri = $"https://{source}.youtrack.cloud/api/issues";
        _httpAuthorization = httpAuthorization;
    }

    public async Task<string> LoadIssues(string query = "fields=id")
    {
        return await MakeRequest($"{_uri}?{query}");
    }

    public async Task<string> LoadTimeTracking(string id, string query = "fields=id")
    {
        return await MakeRequest($"{_uri}/{id}/timeTracking?{query}");
    }

    private async Task<string> MakeRequest(string uri)
    {
        using (var message = new HttpRequestMessage(HttpMethod.Get, uri))
        {
            message.Headers.Authorization = _httpAuthorization.Authorize();
            var response = await _httpClient.SendAsync(message);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}