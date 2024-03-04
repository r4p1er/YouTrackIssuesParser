namespace YouTrackIssuesParser.Models;

public class Comment
{
    public string Id { get; set; }
    public User? Author { get; set; }
    public DateTimeOffset? Created { get; set; }
    public bool Deleted { get; set; }
    public string? Text { get; set; }
}