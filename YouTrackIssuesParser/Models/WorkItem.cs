namespace YouTrackIssuesParser.Models;

public class WorkItem
{
    public string Id { get; set; }
    public User? Author { get; set; }
    public User? Creator { get; set; }
    public string? Text { get; set; }
    public WorkItemType? Type { get; set; }
    public DateTimeOffset Date { get; set; }
    public DurationValue Duration { get; set; }
}