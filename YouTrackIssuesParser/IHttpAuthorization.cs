using System.Net.Http.Headers;

namespace YouTrackIssuesParser;

public interface IHttpAuthorization
{
    AuthenticationHeaderValue Authorize();
}