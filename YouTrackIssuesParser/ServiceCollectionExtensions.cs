using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;

namespace YouTrackIssuesParser;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection collection, string connection)
    {
        collection.AddSingleton<MongoClient>(provider =>
            new MongoClient(connection));
        
        collection.AddSingleton<IMongoDatabase>(provider =>
        {
            var client = provider.GetRequiredService<MongoClient>();
            return client.GetDatabase("YouTrackIssuesParser");
        });
        
        collection.AddSingleton<IMongoCollection<BsonDocument>>(provider =>
        {
            var db = provider.GetRequiredService<IMongoDatabase>();
            return db.GetCollection<BsonDocument>("Issues");
        });

        collection.AddTransient<DbContext>();

        return collection;
    }

    public static IServiceCollection AddYoutrack(this IServiceCollection collection, string source, string token)
    {
        collection.AddSingleton<HttpClient>();
        
        collection.AddTransient<IHttpAuthorization, JwtAuthorization>(provider =>
            new JwtAuthorization(token));
        
        collection.AddTransient<YoutrackClient>(provider =>
        {
            var client = provider.GetRequiredService<HttpClient>();
            var auth = provider.GetRequiredService<IHttpAuthorization>();
            return new YoutrackClient(client, source, auth);
        });

        return collection;
    }
}