namespace YouTrackIssuesParser.Models;

/// <summary>
/// Представляет часть WorkLog`а задачи
/// </summary>
public class WorkItem
{
    /// <summary>
    /// Идентификатор WorkItem`а
    /// </summary>
    public string Id { get; set; }
    
    /// <summary>
    /// Кому назначен WorkItem
    /// </summary>
    public User? Author { get; set; }
    
    /// <summary>
    /// Кто создал WorkItem
    /// </summary>
    public User? Creator { get; set; }
    
    /// <summary>
    /// Описание WorkItem`а
    /// </summary>
    public string? Text { get; set; }
    
    /// <summary>
    /// Тип WorkItem`а
    /// </summary>
    public WorkItemType? Type { get; set; }
    
    /// <summary>
    /// Дата и время WorkItem`а в миллисекундах с эпохи UNIX
    /// </summary>
    public long Date { get; set; }
    
    /// <summary>
    /// Длительность WorkItem`а
    /// </summary>
    public DurationValue Duration { get; set; }
}