namespace YouTrackIssuesParser.Models;

/// <summary>
/// Модель длительности
/// </summary>
public class DurationValue
{
    /// <summary>
    /// Идентификатор длительности
    /// </summary>
    public string Id { get; set; }
    
    /// <summary>
    /// Длительность в минутах
    /// </summary>
    public int Minutes { get; set; }
    
    
    /// <summary>
    /// Строковое представление длительности
    /// </summary>
    public string Presentation { get; set; }
}