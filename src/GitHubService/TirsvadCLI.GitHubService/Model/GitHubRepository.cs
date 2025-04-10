using System.Text.Json.Serialization;

namespace TirsvadCLI.GitHubService.Model;

public class GitHubRepository
{
    public string? Name { get; set; } ///> The name of the repository.
    [JsonPropertyName("html_url")]
    public string? HtmlUrl { get; set; } ///> The URL to the repository on GitHub.
    [JsonPropertyName("clone_url")]
    public string? CloneUrl { get; set; } ///> The URL to clone the repository.
}
