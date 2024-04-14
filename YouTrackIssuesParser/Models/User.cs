namespace YouTrackIssuesParser.Models;

/// <summary>
/// Модель пользователя
/// </summary>
public class User
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public string Id { get; set; }
    
    /// <summary>
    /// Логин пользователя
    /// </summary>
    public string Login { get; set; }
    
    /// <summary>
    /// Полное имя пользователя
    /// </summary>
    public string FullName { get; set; }
    
    /// <summary>
    /// Адрес электронной почты пользователя
    /// </summary>
    public string? Email { get; set; }
}