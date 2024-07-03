using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;

namespace OTest.Services
{
    public class ReqResService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ReqResService> _logger;

        public ReqResService(HttpClient httpClient, ILogger<ReqResService> logger)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://reqres.in/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _logger = logger;
        }

        public async Task<string> GetUsers(int page = 2)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"api/users?page={page}");
                response.EnsureSuccessStatusCode();
                _logger.LogInformation("Successfully fetched users from ReqRes API.");
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching users from ReqRes API.");
                throw;
            }
        }

        public async Task<string> CreateUser(string name, string job)
        {
            try
            {
                var user = new { name = name, job = job };
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/users", user);
                response.EnsureSuccessStatusCode();
                _logger.LogInformation("Successfully created a new user in ReqRes API.");
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new user in ReqRes API.");
                throw;
            }
        }

        public async Task<string> LoginUser(string email, string password)
        {
            var user = new { email = email, password = HashPassword(password) };
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/login", user);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
