using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components;

namespace CDNSBlazorApp.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;
        private string? _token;

        public AuthService(HttpClient httpClient, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
        }

        public bool IsAuthenticated => !string.IsNullOrEmpty(_token);
        public string? UserName { get; private set; }
        public string? FullName { get; private set; }
        public List<string>? Roles { get; private set; }

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/auth/login", new
                {
                    username,
                    password
                });

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    if (result != null)
                    {
                        _token = result.Token;
                        UserName = result.Username;
                        FullName = result.FullName;
                        Roles = result.Roles;

                        _httpClient.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", _token);

                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public void Logout()
        {
            _token = null;
            UserName = null;
            FullName = null;
            Roles = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;
            _navigationManager.NavigateTo("/login");
        }

        public bool HasRole(string role)
        {
            return Roles?.Contains(role) ?? false;
        }

        private class LoginResponse
        {
            public string Token { get; set; } = string.Empty;
            public string Username { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? FullName { get; set; }
            public List<string> Roles { get; set; } = new();
            public int ExpiresIn { get; set; }
        }
    }
}
