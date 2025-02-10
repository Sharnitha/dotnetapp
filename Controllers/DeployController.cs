using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace YourApp.Controllers
{
    public class DeployController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        // Constructor to inject HttpClient and IConfiguration
        public DeployController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        // Method to fetch branches from GitHub repository
        public async Task<IActionResult> Index()
        {
            var branches = await GetBranchesFromGitHub("Sharnitha", "dotnetapp");
            ViewData["Branches"] = branches;
            return View();
        }

        // Method to trigger the pipeline in GitHub Actions
        [HttpPost]
        public async Task<IActionResult> RunPipeline(string branch)
        {
            if (string.IsNullOrEmpty(branch))
            {
                return BadRequest("Branch is required.");
            }

            // Retrieve the GitHub token from environment variable
            var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN"); // Fetch token from environment
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("GitHub token is missing.");
            }

            var success = await TriggerGitHubPipeline("your_github_owner", "your_repo_name", branch, token);

            if (success)
            {
                return RedirectToAction("Index");
            }

            return BadRequest("Failed to trigger pipeline.");
        }

        // Get branches from GitHub repository
        private async Task<List<string>> GetBranchesFromGitHub(string owner, string repo)
        {
            var url = $"https://api.github.com/repos/{owner}/{repo}/branches";
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("dotnet-app");

            var response = await _httpClient.GetStringAsync(url);
            var branches = JsonConvert.DeserializeObject<List<Branch>>(response);

            List<string> branchNames = new List<string>();
            foreach (var branch in branches)
            {
                branchNames.Add(branch.Name);
            }

            return branchNames;
        }

        // Trigger GitHub Actions pipeline (workflow)
        private async Task<bool> TriggerGitHubPipeline(string owner, string repo, string branch, string token)
        {
            var url = $"https://api.github.com/repos/{owner}/{repo}/actions/workflows/your_workflow_file.yml/dispatches";
            var requestBody = new
            {
                ref = branch
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("dotnet-app");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsync(url, content);
            return response.IsSuccessStatusCode;
        }
    }

    public class Branch
    {
        public string Name { get; set; }
    }
}
