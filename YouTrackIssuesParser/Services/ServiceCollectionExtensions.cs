using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;

namespace YouTrackIssuesParser.Services;

/// <summary>
/// Расширения коллекции сервисов
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавление сервисов базы данных
    /// </summary>
    /// <param name="collection">Коллекция сервисов</param>
    /// <param name="connection">Строка подключения к базе данных</param>
    /// <returns>Обновленная коллекция сервисов</returns>
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

    /// <summary>
    /// Добавление сервисов для работы с YouTrack
    /// </summary>
    /// <param name="collection">Коллекция сервисов</param>
    /// <param name="source">Название YouTrack репозитория</param>
    /// <param name="token">Jwt для аутентификации в YouTrack репозитории</param>
    /// <returns>Обновленная коллекция сервисов</returns>
    public static IServiceCollection AddYoutrack(this IServiceCollection collection, string source, string token)
    {
        collection.AddSingleton<HttpClient>();
        
        collection.AddTransient<IHttpAuthenticationService, JwtAuthenticationService>(provider =>
            new JwtAuthenticationService(token));
        
        collection.AddTransient<YoutrackClient>(provider =>
        {
            var client = provider.GetRequiredService<HttpClient>();
            var auth = provider.GetRequiredService<IHttpAuthenticationService>();
            return new YoutrackClient(client, source, auth);
        });

        return collection;
    }
}