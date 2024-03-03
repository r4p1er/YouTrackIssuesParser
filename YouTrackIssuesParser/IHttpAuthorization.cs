namespace YouTrackIssuesParser;

public interface IHttpAuthorization
{
    void Authorize(HttpRequestMessage message);
}