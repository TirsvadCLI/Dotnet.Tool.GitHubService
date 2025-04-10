namespace TirsvadCLI.GitHubService.Model;

public class GitHubRepository
{
    public string? Name { get; set; } ///> The name of the repository.
    public string? HtmlUrl { get; set; } ///> The URL to the repository on GitHub.
    public string? CloneUrl { get; set; } ///> The URL to clone the repository.
}
