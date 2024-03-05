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

    public async Task SaveIssuesArray(IEnumerable<Issue> issues)
    {
        var docs = issues.Select(x => x.ToBsonDocument()).ToList();
        await _collection.InsertManyAsync(docs);
    }
}