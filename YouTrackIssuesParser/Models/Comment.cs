namespace YouTrackIssuesParser.Models;

public class Comment
{
    public string Id { get; set; }
    
    public User? Author { get; set; }
    
    public long? Created { get; set; }
    
    public bool Deleted { get; set; }
    
    public string? Text { get; set; }
}