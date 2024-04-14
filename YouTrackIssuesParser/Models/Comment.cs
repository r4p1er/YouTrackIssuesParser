namespace YouTrackIssuesParser.Models;

/// <summary>
/// Модель комментария
/// </summary>
public class Comment
{
    /// <summary>
    /// Идентификатор комментария
    /// </summary>
    public string Id { get; set; }
    
    /// <summary>
    /// Автор комментария
    /// </summary>
    public User? Author { get; set; }
    
    /// <summary>
    /// Время создания комментария в миллисекундах с эпохи UNIX
    /// </summary>
    public long? Created { get; set; }
    
    /// <summary>
    /// Удален ли комментарий
    /// </summary>
    public bool Deleted { get; set; }
    
    /// <summary>
    /// Текст комментария
    /// </summary>
    public string? Text { get; set; }
}