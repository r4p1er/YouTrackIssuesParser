namespace YouTrackIssuesParser.Models;

public class TimeTracking
{
    public string Id { get; set; }
    public bool Enabled { get; set; }
    public List<WorkItem> WorkItems { get; set; } = new();
}