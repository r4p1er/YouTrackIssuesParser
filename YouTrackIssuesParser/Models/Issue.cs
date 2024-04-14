using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace YouTrackIssuesParser.Models;

/// <summary>
/// Модель задачи
/// </summary>
public class Issue
{
    /// <summary>
    /// Идентификатор документа MongoDB
    /// </summary>
    [JsonIgnore]
    [BsonId]
    public ObjectId DocumentId { get; set; }
    
    /// <summary>
    /// Идентификатор задачи
    /// </summary>
    public string Id { get; set; }
    
    /// <summary>
    /// Видимый в интерфейсе идентификатор задачи
    /// </summary>
    [JsonPropertyName("idReadable")]
    public string Key { get; set; }
    
    /// <summary>
    /// Заголовок задачи
    /// </summary>
    [JsonPropertyName("summary")]
    public string? Name { get; set; }
    
    /// <summary>
    /// Описание задачи
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Время последнего обновления задачи в миллисекундах с эпохи UNIX
    /// </summary>
    public long? Updated { get; set; }
    
    /// <summary>
    /// Комментарии задачи
    /// </summary>
    public List<Comment> Comments { get; set; } = new();
    
    /// <summary>
    /// Кому назначена задача
    /// </summary>
    public User? Assignee { get; set; }
    
    /// <summary>
    /// Тип задачи
    /// </summary>
    public string Type { get; set; }
    
    /// <summary>
    /// Статус задачи
    /// </summary>
    public string State { get; set; }
    
    /// <summary>
    /// Приоритет задачи
    /// </summary>
    public string Priority { get; set; }
    
    /// <summary>
    /// Строковое представление общего затраченного на задачу времени
    /// </summary>
    public string? SpentTime { get; set; }
    
    /// <summary>
    /// WorkLog`и задачи
    /// </summary>
    public TimeTracking? WorkLogs { get; set; }
    
    /// <summary>
    /// Строковое представление всех кастомных полей. Не сохраняется в базе данных
    /// </summary>
    [BsonIgnore]
    public JsonArray CustomFields { get; set; }
}