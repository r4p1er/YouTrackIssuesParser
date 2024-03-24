using Microsoft.Extensions.Logging;
using Quartz;

namespace YouTrackIssuesParser;

public class YoutrackParsingJob : IJob
{
    private readonly YoutrackClient _client;
    private readonly Parser _parser;
    private readonly DbContext _context;
    private readonly ILogger<YoutrackParsingJob> _logger;
    
    public YoutrackParsingJob(YoutrackClient client, Parser parser, DbContext context, ILogger<YoutrackParsingJob> logger)
    {
        _client = client;
        _parser = parser;
        _context = context;
        _logger = logger;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Started receiving tasks");
        string issuesJson = await _client.LoadIssues(
            "fields=id,updated,idReadable,summary,description,comments(author(id,login,fullName,email),text,id,created,deleted),customFields(name,value(id,login,fullName,email,name,presentation))");
        _logger.LogInformation("Started parsing tasks");
        var issues = _parser.ParseIssuesArray(issuesJson);
        _logger.LogInformation($"Parsed {issues.Count} tasks");

        _logger.LogInformation("Started updating database");
        foreach (var issue in issues)
        {
            var timeTrackingJson = await _client.LoadTimeTracking(issue.Id,
                "fields=id,enabled,workItems(id,author(id,login,fullName,email),creator(id,login,fullName,email),text,type(id,name),date,duration(id,minutes,presentation))");
            issue.WorkLogs = _parser.ParseIssueWorkLog(timeTrackingJson);
            var document = await _context.FindById(issue.Id);
            
            if (document == null)
            {
                await _context.Add(issue);
            }
            else if (document["Updated"].AsInt64 < issue.Updated)
            {
                await _context.ReplaceById(issue);
            }
        }
        
        _logger.LogInformation("Updated database");
    }
}