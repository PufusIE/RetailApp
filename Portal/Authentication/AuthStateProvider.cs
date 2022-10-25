using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Portal.Authentication
{
    // Checks if user logged in or not right from the beginning 
    public class AuthStateProvider : AuthenticationStateProvider 
    {
        private readonly HttpClient _httpClient ;
        private readonly ILocalStorageService _localStorage;
        private readonly IConfiguration _config;
        private readonly AuthenticationState _anonymous;

        public AuthStateProvider(HttpClient httpClient, ILocalStorageService localStorage, IConfiguration config)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _config = config;
            _anonymous = new AuthenticationState(new ClaimsPrincipal( new ClaimsIdentity()));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Tries to get the token from local storage(cash)
            var tokenStorageLocationKey = _config["authTokenStorageKey"];
            var token = await _localStorage.GetItemAsync<string>(tokenStorageLocationKey);

            if (string.IsNullOrWhiteSpace(token))
            {
                return _anonymous;
            }

            // Sends this token with each future api request 
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

            // Returns authenticated user
            return new AuthenticationState(
                new ClaimsPrincipal(
                    new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType")));
        }

        // Trigger Authentication event that changes the login status of the current user
        public void NotifyAuthentication(string token)
        {
            var authenticatedUser = new ClaimsPrincipal(
                    new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType")) ;

            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));

            // New authentication state
            NotifyAuthenticationStateChanged(authState);
        }

        // Notify that the current user is logged out
        public void NotifyLogout()
        {
            // Resets the state of the current authenticated user
            var authState = Task.FromResult(_anonymous);
            NotifyAuthenticationStateChanged(authState);
        }
    }
}
