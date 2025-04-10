using System.Text.Json;
using TirsvadCLI.GitHubService.Model;

namespace TirsvadCLI.GitHubService;

public class GitHubService
{
    private readonly HttpClient _httpClient;

    public GitHubService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        // GitHub API requires a user agent. You can set this to your application's name.
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("MyAppGitHubClient/1.0");

        // Optional: If you need to use authentication because you're accessing private repos
        // or want higher rate limits, uncomment and set your personal access token.
        // _httpClient.DefaultRequestHeaders.Authorization =
        //     new AuthenticationHeaderValue("token", "YOUR_GITHUB_PERSONAL_ACCESS_TOKEN");
    }

    /// <summary>
    /// Retrieves all repositories for a given organization.
    /// </summary>
    /// <param name="organization">The GitHub organization name.</param>
    /// <returns>A list of GitHubRepository objects.</returns>
    public async Task<List<GitHubRepository>> GetOrganizationRepositoriesAsync(string organization)
    {
        List<GitHubRepository> repositories = new List<GitHubRepository>();
        int perPage = 100;
        int page = 1;
        bool hasMore = true;

        while (hasMore)
        {
            // Construct the API URL with pagination parameters.
            var url = $"https://api.github.com/orgs/{organization}/repos?per_page={perPage}&page={page}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error fetching repositories: {response.ReasonPhrase}");
                return repositories;
            }

            var content = await response.Content.ReadAsStringAsync();

            try
            {
                // Deserialize the JSON array into a list of repositories.
                var reposPage = JsonSerializer.Deserialize<List<GitHubRepository>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (reposPage == null || reposPage.Count == 0)
                {
                    hasMore = false; // Exit the loop when no more items are returned.
                }
                else
                {
                    // Check if the current page's content is identical to the previous page's content.
                    if (repositories.Any() && repositories.TakeLast(perPage).SequenceEqual(reposPage))
                    {
                        Console.WriteLine("Duplicate page detected. Stopping further requests.");
                        hasMore = false;
                    }
                    else
                    {
                        repositories.AddRange(reposPage);
                        page++; // Move to the next page.
                    }
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing response content: {ex.Message}");
                return repositories;
            }
        }

        return repositories;
    }

}
