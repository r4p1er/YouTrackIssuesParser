using Microsoft.Extensions.Logging;
using Quartz;

namespace YouTrackIssuesParser;

public class MainJob : IJob
{
    private readonly IssuesLoader _loader;
    private readonly IssuesParser _parser;
    private readonly IssuesDbContext _context;
    private readonly ILogger<MainJob> _logger;
    
    public MainJob(IssuesLoader loader, IssuesParser parser, IssuesDbContext context, ILogger<MainJob> logger)
    {
        _loader = loader;
        _parser = parser;
        _context = context;
        _logger = logger;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Started receiving tasks");
        string issuesJson = await _loader.LoadIssues(
            "fields=id,updated,idReadable,summary,description,comments(author(id,login,fullName,email),text,id,created,deleted),customFields(name,value(id,login,fullName,email,name,presentation))");
        _logger.LogInformation("Started parsing tasks");
        var issues = _parser.ParseIssuesArray(issuesJson);
        _logger.LogInformation($"Parsed {issues.Count} tasks");

        _logger.LogInformation("Started updating database");
        foreach (var issue in issues)
        {
            var timeTrackingJson = await _loader.LoadTimeTracking(issue.Id,
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