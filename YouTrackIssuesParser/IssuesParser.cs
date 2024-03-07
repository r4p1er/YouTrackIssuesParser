using System.Text.Json;
using YouTrackIssuesParser.Models;

namespace YouTrackIssuesParser;

public class IssuesParser
{
    public List<Issue> ParseIssuesArray(string json)
    {
        var result = new List<Issue>();
        
        using (var document = JsonDocument.Parse(json))
        {
            foreach (var item in document.RootElement.EnumerateArray())
            {
                var issue = new Issue()
                {
                    Id = item.GetProperty("id").GetString()!,
                    Key = item.GetProperty("idReadable").GetString()!,
                    Name = item.GetProperty("summary").GetString(),
                    Description = item.GetProperty("description").GetString(),
                    Updated = item.GetProperty("updated").ValueKind != JsonValueKind.Null
                        ? item.GetProperty("updated").GetInt64()
                        : null
                };

                foreach (var commentItem in item.GetProperty("comments").EnumerateArray())
                {
                    var comm = new Comment()
                    {
                        Id = commentItem.GetProperty("id").GetString()!,
                        Created = commentItem.GetProperty("created").GetInt64(),
                        Deleted = commentItem.GetProperty("deleted").GetBoolean(),
                        Text = commentItem.GetProperty("text").GetString()
                    };
                    if (commentItem.GetProperty("author").ValueKind != JsonValueKind.Null)
                    {
                        comm.Author = new User()
                        {
                            Id = commentItem.GetProperty("author").GetProperty("id").GetString()!,
                            Login = commentItem.GetProperty("author").GetProperty("login").GetString()!,
                            FullName = commentItem.GetProperty("author").GetProperty("fullName").GetString()!,
                            Email = commentItem.GetProperty("author").GetProperty("email").GetString()
                        };
                    }
                    else
                    {
                        comm.Author = null;
                    }
                    issue.Comments.Add(comm);
                }

                foreach (var custom in item.GetProperty("customFields").EnumerateArray())
                {
                    if (custom.GetProperty("name").GetString()! == "Priority")
                    {
                        issue.Priority = custom.GetProperty("value").GetProperty("name").GetString()!;
                    }

                    if (custom.GetProperty("name").GetString()! == "Type")
                    {
                        issue.Type = custom.GetProperty("value").GetProperty("name").GetString()!;
                    }

                    if (custom.GetProperty("name").GetString()! == "State")
                    {
                        issue.State = custom.GetProperty("value").GetProperty("name").GetString()!;
                    }

                    if (custom.GetProperty("name").GetString()! == "Assignee")
                    {
                        if (custom.GetProperty("value").ValueKind != JsonValueKind.Null)
                        {
                            issue.Assignee = new User()
                            {
                                Id = custom.GetProperty("value").GetProperty("id").GetString()!,
                                Login = custom.GetProperty("value").GetProperty("login").GetString()!,
                                FullName = custom.GetProperty("value").GetProperty("fullName").GetString()!,
                                Email = custom.GetProperty("value").GetProperty("email").GetString()
                            };
                        }
                        else
                        {
                            issue.Assignee = null;
                        }
                    }

                    if (custom.GetProperty("name").GetString()! == "Spent time")
                    {
                        issue.SpentTime = custom.GetProperty("value").ValueKind != JsonValueKind.Null ? custom.GetProperty("value").GetProperty("presentation").GetString() : null;
                    }
                }

                issue.WorkLogs = null;
                
                result.Add(issue);
            }
        }

        return result;
    }

    public TimeTracking ParseIssueWorkLog(string json)
    {
        using (var document = JsonDocument.Parse(json))
        {
            var workLog = new TimeTracking()
            {
                Id = document.RootElement.GetProperty("id").GetString()!,
                Enabled = document.RootElement.GetProperty("enabled").GetBoolean(),
                WorkItems = new()
            };

            foreach (var item in document.RootElement.GetProperty("workItems").EnumerateArray())
            {
                var workItem = new WorkItem()
                {
                    Id = item.GetProperty("id").GetString()!,
                    Text = item.GetProperty("text").GetString(),
                    Date = item.GetProperty("date").GetInt64(),
                    Duration = new DurationValue()
                    {
                        Id = item.GetProperty("duration").GetProperty("id").GetString()!,
                        Minutes = item.GetProperty("duration").GetProperty("minutes").GetInt32(),
                        Presentation = item.GetProperty("duration").GetProperty("presentation").GetString()!
                    }
                };

                if (item.GetProperty("type").ValueKind != JsonValueKind.Null)
                {
                    workItem.Type = new WorkItemType()
                    {
                        Id = item.GetProperty("type").GetProperty("id").GetString()!,
                        Name = item.GetProperty("type").GetProperty("name").GetString()!
                    };
                }
                else
                {
                    workItem.Type = null;
                }

                if (item.GetProperty("author").ValueKind != JsonValueKind.Null)
                {
                    workItem.Author = new User()
                    {
                        Id = item.GetProperty("author").GetProperty("id").GetString()!,
                        Login = item.GetProperty("author").GetProperty("login").GetString()!,
                        FullName = item.GetProperty("author").GetProperty("fullName").GetString()!,
                        Email = item.GetProperty("author").GetProperty("email").GetString()
                    };
                }
                else
                {
                    workItem.Author = null;
                }

                if (item.GetProperty("creator").ValueKind != JsonValueKind.Null)
                {
                    workItem.Creator = new User()
                    {
                        Id = item.GetProperty("creator").GetProperty("id").GetString()!,
                        Login = item.GetProperty("creator").GetProperty("login").GetString()!,
                        FullName = item.GetProperty("creator").GetProperty("fullName").GetString()!,
                        Email = item.GetProperty("creator").GetProperty("email").GetString()
                    };
                }
                else
                {
                    workItem.Creator = null;
                }
                
                workLog.WorkItems.Add(workItem);
            }

            return workLog;
        }
    }
}