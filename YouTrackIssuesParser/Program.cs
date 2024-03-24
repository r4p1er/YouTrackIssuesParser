using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Quartz;

namespace YouTrackIssuesParser;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddLogging(logging => logging.AddConsole());
        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddTransient<IHttpAuthorization, JwtAuthorization>(provider =>
            new JwtAuthorization(builder.Configuration["Authentication:Jwt"]!));
        builder.Services.AddTransient<IssuesLoader>(provider =>
        {
            var client = provider.GetRequiredService<HttpClient>();
            var auth = provider.GetRequiredService<IHttpAuthorization>();
            return new IssuesLoader(client, builder.Configuration["Source"]!, auth);
        });
        builder.Services.AddTransient<IssuesParser>();
        builder.Services.AddSingleton<MongoClient>(provider =>
            new MongoClient(builder.Configuration["Mongo:ConnectionString"]));
        builder.Services.AddSingleton<IMongoDatabase>(provider =>
        {
            var client = provider.GetRequiredService<MongoClient>();
            return client.GetDatabase("YouTrackIssuesParser");
        });
        builder.Services.AddSingleton<IMongoCollection<BsonDocument>>(provider =>
        {
            var db = provider.GetRequiredService<IMongoDatabase>();
            return db.GetCollection<BsonDocument>("Issues");
        });
        builder.Services.AddTransient<IssuesDbContext>();
        builder.Services.AddQuartz();
        builder.Services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });
        
        var schedulerFactory = builder.Services.BuildServiceProvider().GetRequiredService<ISchedulerFactory>();
        var scheduler = await schedulerFactory.GetScheduler();

        var job = JobBuilder.Create<MainJob>()
            .WithIdentity("mainJob", "group1")
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity("mainTrigger", "group1")
            .WithCronSchedule(builder.Configuration["QuartzCronExpression"]!)
            .Build();

        await scheduler.ScheduleJob(job, trigger);
        
        var host = builder.Build();
        await host.RunAsync();
    }
}