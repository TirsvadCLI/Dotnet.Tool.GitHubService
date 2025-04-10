using TirsvadCLI.GitHubService;

namespace Example;

internal class Program
{
    static async Task Main(string[] args)
    {
        var gitHubService = new GitHubService(new HttpClient());
        var actualRepositories = await gitHubService.GetOrganizationRepositoriesAsync("test-org");
        Console.WriteLine("Number of repositories: " + actualRepositories.Count);
        foreach (var repo in actualRepositories)
        {
            // Uncomment the following lines to print the repository details
            //// Print the repository details
            // Console.WriteLine($"Name: {repo.Name}, URL: {repo.HtmlUrl}, Clone URL: {repo.CloneUrl}");

            // For demonstration, we will just print the name
            Console.WriteLine($"Name: {repo.Name}");

            // Uncomment the following lines to clone the repository
            //// Clone the repository using its name into %user%\source\repo
            //var userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            //var targetFolder = Path.Combine(userFolder, "source", "repo");
            //Directory.CreateDirectory(targetFolder);

            //var cloneCommand = $"git clone {repo.CloneUrl} \"{Path.Combine(targetFolder, repo.Name)}\"";
            //Console.WriteLine($"Executing: {cloneCommand}");
            //System.Diagnostics.Process.Start("cmd.exe", $"/c {cloneCommand}");
        }
    }
}
