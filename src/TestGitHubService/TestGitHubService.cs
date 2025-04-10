using System.Net;
using Moq;
using Moq.Protected;
using TirsvadCLI.GitHubService;
using TirsvadCLI.GitHubService.Model;

namespace TestGitHubService;

[TestClass]
public sealed class TestGitHubService
{
    [TestClass]
    public class GitHubServiceTests
    {
        private const string _errorOrgName = "error-org";

        private const string _validOrgName = "test-org";

        [TestMethod]
        public async Task GetOrganizationRepositoriesAsync_ReturnsRepositories()
        {
            // Arrange
            var _validOrgName = "test-org"; // Ensure this matches the organization name in the URLs
            var expectedRepositories = new List<GitHubRepository>
            {
                new GitHubRepository { Name = "test-proj", HtmlUrl = $"https://github.com/{_validOrgName}/test-proj", CloneUrl = $"https://github.com/{_validOrgName}/test-proj.git" },
                new GitHubRepository { Name = "example", HtmlUrl = $"https://github.com/{_validOrgName}/example", CloneUrl = $"https://github.com/{_validOrgName}/example.git" },
            };

            var gitHubService = new GitHubService(new HttpClient());

            // Act
            var actualRepositories = await gitHubService.GetOrganizationRepositoriesAsync(_validOrgName);

            // Assert
            Assert.IsNotNull(actualRepositories);
            Assert.AreEqual(expectedRepositories.Count, actualRepositories.Count);
            Assert.AreEqual(expectedRepositories[0].Name, actualRepositories[0].Name);
            Assert.AreEqual(expectedRepositories[1].HtmlUrl, actualRepositories[1].HtmlUrl);
        }


        [TestMethod]
        public async Task GetOrganizationRepositoriesAsync_ReturnsEmptyList_WhenNoRepositories()
        {
            // Arrange
            var organization = "empty-org";

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString().StartsWith($"https://api.github.com/orgs/{organization}/repos")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[]") // Empty JSON array
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var gitHubService = new GitHubService(httpClient);

            // Act
            var actualRepositories = await gitHubService.GetOrganizationRepositoriesAsync(organization);

            // Assert
            Assert.IsNotNull(actualRepositories);
            Assert.AreEqual(0, actualRepositories.Count);
        }

        [TestMethod]
        public async Task GetOrganizationRepositoriesAsync_HandlesErrorResponse()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri != null && // Ensure RequestUri is not null
                        req.RequestUri.ToString().StartsWith($"https://api.github.com/orgs/{_errorOrgName}/repos")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    ReasonPhrase = "Not Found"
                });

            var httpClient = new HttpClient();
            var gitHubService = new GitHubService(httpClient);

            // Act
            var actualRepositories = await gitHubService.GetOrganizationRepositoriesAsync(_errorOrgName);

            // Assert
            Assert.IsNotNull(actualRepositories);
            Assert.AreEqual(0, actualRepositories.Count); // Should return an empty list on error
        }
    }
}
