namespace YouTrackIssuesParser.Models;

public class User
{
    public string Id { get; set; }
    public string Login { get; set; }
    public string FullName { get; set; }
    public string? Email { get; set; }
}