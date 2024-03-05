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

    public async Task<BsonDocument> FindById(string id)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("id", id);
        return await _collection.FindAsync(filter).Result.FirstOrDefaultAsync();
    }

    public async Task ReplaceById(Issue issue)
    {
        var bson = await FindById(issue.Id);
        await _collection.ReplaceOneAsync(bson, issue.ToBsonDocument());
    }

    public async Task DeleteById(string id)
    {
        var bson = await FindById(id);
        await _collection.DeleteOneAsync(bson);
    }
}