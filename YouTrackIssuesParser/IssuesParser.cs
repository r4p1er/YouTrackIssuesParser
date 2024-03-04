﻿using System.Text.Json;
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
                };

                foreach (var commentItem in item.GetProperty("comments").EnumerateArray())
                {
                    var comm = new Comment()
                    {
                        Id = commentItem.GetProperty("id").GetString()!,
                        Created =
                            DateTimeOffset.FromUnixTimeMilliseconds(commentItem.GetProperty("created").GetInt64()),
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
}