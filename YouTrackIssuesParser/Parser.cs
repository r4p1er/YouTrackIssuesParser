using System.Text.Json;
using YouTrackIssuesParser.Models;

namespace YouTrackIssuesParser;

/// <summary>
/// Парсер для YouTrack
/// </summary>
public class Parser
{
    /// <summary>
    /// Парсинг кастомных полей у набора задач
    /// </summary>
    /// <param name="issues">Набор задач</param>
    public void ParseIssuesCustomFields(IEnumerable<Issue> issues)
    {
        foreach (var issue in issues)
        {
            foreach (var item in issue.CustomFields)
            {
                switch (item!["name"]!.GetValue<string>())
                {
                    case "Priority":
                        issue.Priority = item["value"]!["name"]!.GetValue<string>();
                        break;
                    case "Type":
                        issue.Type = item["value"]!["name"]!.GetValue<string>();
                        break;
                    case "State":
                        issue.State = item["value"]!["name"]!.GetValue<string>();
                        break;
                    case "Assignee":
                        issue.Assignee = item["value"]?.Deserialize<User>(new JsonSerializerOptions
                            { PropertyNameCaseInsensitive = true });
                        break;
                    case "Spent time":
                        issue.SpentTime = item["value"]?["presentation"]!.GetValue<string>();
                        break;
                }
            }
        }
    }
}