using Microsoft.Extensions.Logging;
using Quartz;
using YouTrackIssuesParser.Models;

namespace YouTrackIssuesParser.Jobs;

/// <summary>
/// Работа по парсингу задач с YouTrack
/// </summary>
public class YoutrackParsingJob : IJob
{
    /// <inheritdoc cref="YoutrackClient"/>
    private readonly YoutrackClient _client;
    
    /// <inheritdoc cref="Parser"/>
    private readonly Parser _parser;
    
    /// <inheritdoc cref="DbContext"/>
    private readonly DbContext _context;
    
    /// <inheritdoc cref="ILogger"/>
    private readonly ILogger<YoutrackParsingJob> _logger;
    
    public YoutrackParsingJob(YoutrackClient client, Parser parser, DbContext context, ILogger<YoutrackParsingJob> logger)
    {
        _client = client;
        _parser = parser;
        _context = context;
        _logger = logger;
    }
    
    /// <summary>
    /// Выполнить работу
    /// </summary>
    /// <param name="context">Контекст работы</param>
    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Started receiving tasks");
        var issues = await _client.LoadIssues(
            "fields=id,updated,idReadable,summary,description,comments(author(id,login,fullName,email),text,id,created,deleted),customFields(name,value(id,login,fullName,email,name,presentation))");
        _logger.LogInformation($"Received {issues.Count} tasks");
        _logger.LogInformation("Started parsing custom fields");
        _parser.ParseIssuesCustomFields(issues);
        _logger.LogInformation("Parsed custom fields");

        _logger.LogInformation("Started updating database");
        Parallel.ForEach(issues, async issue =>
        {
            issue.WorkLogs = await _client.LoadTimeTracking(issue.Id,
                "fields=id,enabled,workItems(id,author(id,login,fullName,email),creator(id,login,fullName,email),text,type(id,name),date,duration(id,minutes,presentation))");
            var document = await _context.FindById(issue.Id);

            if (document == null)
            {
                await _context.Add(issue);
            }
            else if (document["Updated"].AsInt64 < issue.Updated)
            {
                await _context.ReplaceById(issue);
            }
        });
        
        _logger.LogInformation("Updated database");
    }
}