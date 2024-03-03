namespace YouTrackIssuesParser;

public class IssuesLoader
{
    private readonly HttpClient httpClient;
    private readonly IHttpAuthorization httpAuthorization;

    public IssuesLoader(HttpClient httpClient, IHttpAuthorization httpAuthorization)
    {
        this.httpClient = httpClient;
        this.httpAuthorization = httpAuthorization;
    }

    public async Task<string> Load(string source, string query = "fields=id")
    {
        using (var message =
               new HttpRequestMessage(HttpMethod.Get, $"https://{source}.youtrack.cloud/api/issues?{query}"))
        {
            httpAuthorization.Authorize(message);
            var response = await httpClient.SendAsync(message);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}