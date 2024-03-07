using Quartz;

namespace YouTrackIssuesParser;

public class MainJob : IJob
{
    private readonly IssuesLoader _loader;
    private readonly IssuesParser _parser;
    private readonly IssuesDbContext _context;
    
    public MainJob(IssuesLoader loader, IssuesParser parser, IssuesDbContext context)
    {
        _loader = loader;
        _parser = parser;
        _context = context;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        string issuesJson = await _loader.LoadIssues(
            "fields=id,updated,idReadable,summary,description,comments(author(id,login,fullName,email),text,id,created,deleted),customFields(name,value(id,login,fullName,email,name,presentation))");
        var issues = _parser.ParseIssuesArray(issuesJson);

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
    }
}