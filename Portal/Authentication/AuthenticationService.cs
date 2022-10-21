using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Portal.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Portal.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthenticationService(HttpClient httpClient, AuthenticationStateProvider authProvider, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _authProvider = authProvider;
            _localStorage = localStorage;
        }

        public async Task<AuthenticatedUserModel> Login(AuthenticationUserModel userForAuthentication)
        {
            // Populate what's going to be sent to /token endpoint
            var data = new TokenUserModel
            {
                Username = userForAuthentication.Email,
                Password = userForAuthentication.Password,
                Grant_Type = "password"
            };

            // Calling /token
            var authResult = await _httpClient.PostAsJsonAsync("https://localhost:7054/token", data);
            // Reading response (token and username)
            var authContent = await authResult.Content.ReadAsStringAsync();

            if (authResult.IsSuccessStatusCode == false)
            {
                return null;
            }

            // Converting response from json to our model
            var result = JsonSerializer.Deserialize<AuthenticatedUserModel>(authContent);

            // Cashing token to user's machine 
            await _localStorage.SetItemAsync("authToken", result.Access_Token);

            // Casting authstate provider from microsoft with own child class and notifying authentication
            ((AuthStateProvider)_authProvider).NotifyAuthentication(result.Access_Token);

            // Sends this token with each future api request 
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.Access_Token);

            return result;
        }

        public async Task Logout()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
            await _localStorage.RemoveItemAsync("authToken");
            ((AuthStateProvider)_authProvider).NotifyLogout();
        }
    }
}
