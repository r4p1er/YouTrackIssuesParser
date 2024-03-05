namespace YouTrackIssuesParser.Models;

public class Issue
{
    public string Id { get; set; }
    public string Key { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset? Updated { get; set; }
    public List<Comment> Comments { get; set; } = new();
    public User? Assignee { get; set; }
    public string Type { get; set; }
    public string State { get; set; }
    public string Priority { get; set; }
    public string? SpentTime { get; set; }
    public TimeTracking? WorkLogs { get; set; }
}