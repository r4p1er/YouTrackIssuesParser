using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using YouTrackIssuesParser.Jobs;
using YouTrackIssuesParser.Services;

namespace YouTrackIssuesParser;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddLogging(logging => logging.AddConsole());
        builder.Services.AddYoutrack(builder.Configuration["Source"]!, builder.Configuration["Authentication:Jwt"]!);
        builder.Services.AddTransient<Parser>();
        builder.Services.AddDatabase(builder.Configuration["Mongo:ConnectionString"]!);
        builder.Services.AddQuartz();
        builder.Services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });
        
        var schedulerFactory = builder.Services.BuildServiceProvider().GetRequiredService<ISchedulerFactory>();
        var scheduler = await schedulerFactory.GetScheduler();

        var job = JobBuilder.Create<YoutrackParsingJob>()
            .WithIdentity("youtrackParsingJob", "group1")
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity("youtrackParsingTrigger", "group1")
            .WithCronSchedule(builder.Configuration["QuartzCronExpression"]!)
            .Build();

        await scheduler.ScheduleJob(job, trigger);
        
        var host = builder.Build();
        await host.RunAsync();
    }
}