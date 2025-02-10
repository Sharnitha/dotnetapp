using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyDotNetApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeployController : ControllerBase
    {
        private readonly string githubToken = "";  // GitHub Token
        private readonly string owner = "Sharnitha";
        private readonly string repo = "dotnetapp";

        [HttpPost]
        [Route("trigger-deploy")]
        public async Task<IActionResult> TriggerDeploy([FromBody] DeployRequest request)
        {
            var url = $"https://api.github.com/repos/{owner}/{repo}/actions/workflows/deploy.yml/dispatches";
            
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "HttpClient");
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {githubToken}");
                client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");

                var payload = new
                {
                    ref = request.Branch,  // Branch to deploy
                    inputs = new
                    {
                        branch = request.Branch  // Send the branch info to GitHub Actions
                    }
                };

                var jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    return Ok("Deployment triggered successfully.");
                }
                else
                {
                    return BadRequest($"Failed to trigger deployment. Status: {response.StatusCode}");
                }
            }
        }
    }

    public class DeployRequest
    {
        public string Branch { get; set; }
    }
}
