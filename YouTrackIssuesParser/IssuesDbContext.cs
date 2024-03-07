using MongoDB.Bson;
using MongoDB.Driver;
using YouTrackIssuesParser.Models;

namespace YouTrackIssuesParser;

public class IssuesDbContext
{
    private readonly IMongoCollection<BsonDocument> _collection;

    public IssuesDbContext(IMongoCollection<BsonDocument> collection)
    {
        _collection = collection;
    }

    public async Task Add(Issue issue)
    {
        await _collection.InsertOneAsync(issue.ToBsonDocument());
    }

    public async Task AddRange(IEnumerable<Issue> issues)
    {
        var docs = issues.Select(x => x.ToBsonDocument()).ToList();
        await _collection.InsertManyAsync(docs);
    }

    public async Task<BsonDocument?> FindById(string id)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("Id", id);
        return await (await _collection.FindAsync(filter)).FirstOrDefaultAsync();
    }

    public async Task ReplaceById(Issue issue)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("Id", issue.Id);
        var doc = issue.ToBsonDocument();
        doc.Remove("_id");
        await _collection.ReplaceOneAsync(filter, doc);
    }

    public async Task DeleteById(string id)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("Id", id);
        await _collection.DeleteOneAsync(filter);
    }
}