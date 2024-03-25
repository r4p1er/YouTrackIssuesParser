namespace YouTrackIssuesParser.Models;

/// <summary>
/// Представляет WorkLog`и задачи. Позволяет получить WorkItem`ы задачи
/// </summary>
public class TimeTracking
{
    /// <summary>
    /// Идентификатор TimeTracking
    /// </summary>
    public string Id { get; set; }
    
    /// <summary>
    /// Показывает включен ли TimeTracking для проекта, в который входит данная задача
    /// </summary>
    public bool Enabled { get; set; }
    
    /// <summary>
    /// WorkItem`ы задачи
    /// </summary>
    public List<WorkItem> WorkItems { get; set; } = new();
}