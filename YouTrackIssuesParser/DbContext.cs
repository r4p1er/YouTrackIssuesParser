using MongoDB.Bson;
using MongoDB.Driver;
using YouTrackIssuesParser.Models;

namespace YouTrackIssuesParser;

/// <summary>
/// Контекст базы данных. Позволяет добавить, получить, обновить и удалить задачи
/// </summary>
public class DbContext
{
    /// <summary>
    /// Коллекция задач в базе данных
    /// </summary>
    private readonly IMongoCollection<BsonDocument> _collection;
    
    public DbContext(IMongoCollection<BsonDocument> collection)
    {
        _collection = collection;
    }

    /// <summary>
    /// Добавление задачи
    /// </summary>
    /// <param name="issue">Модель задачи</param>
    public async Task Add(Issue issue)
    {
        await _collection.InsertOneAsync(issue.ToBsonDocument());
    }

    /// <summary>
    /// Добавление набора задач
    /// </summary>
    /// <param name="issues">Набор задач</param>
    public async Task AddRange(IEnumerable<Issue> issues)
    {
        var docs = issues.Select(x => x.ToBsonDocument()).ToList();
        await _collection.InsertManyAsync(docs);
    }

    /// <summary>
    /// Получение задачи по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор задачи</param>
    /// <returns>Представляющий задачу документ MongoDB</returns>
    public async Task<BsonDocument?> FindById(string id)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("Id", id);
        return await (await _collection.FindAsync(filter)).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Обновление задачи по идентификатору
    /// </summary>
    /// <param name="issue">Обновленная задача</param>
    public async Task ReplaceById(Issue issue)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("Id", issue.Id);
        var doc = issue.ToBsonDocument();
        doc.Remove("_id");
        await _collection.ReplaceOneAsync(filter, doc);
    }

    /// <summary>
    /// Удаление задачи по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор задачи</param>
    public async Task DeleteById(string id)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("Id", id);
        await _collection.DeleteOneAsync(filter);
    }
}