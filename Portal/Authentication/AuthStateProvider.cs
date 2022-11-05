using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using RAWPFDesktopUILibrary.Api;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Portal.Authentication
{      
    public class AuthStateProvider : AuthenticationStateProvider 
    {
        private readonly HttpClient _httpClient ;
        private readonly ILocalStorageService _localStorage;
        private readonly IConfiguration _config;
        private readonly IAPIHelper _apiHelper;
        // For cases when you want to log off user
        private readonly AuthenticationState _anonymous;

        public AuthStateProvider(HttpClient httpClient,
                                 ILocalStorageService localStorage,
                                 IConfiguration config,
                                 IAPIHelper apiHelper)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _config = config;
            _apiHelper = apiHelper;
            _anonymous = new AuthenticationState(new ClaimsPrincipal( new ClaimsIdentity()));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var tokenStorageLocationKey = _config["authTokenStorageKey"];
            var token = await _localStorage.GetItemAsync<string>(tokenStorageLocationKey);

            if (string.IsNullOrWhiteSpace(token))
            {
                return _anonymous;
            }

            bool isAuthenticated = await NotifyAuthentication(token);

            if (isAuthenticated == false)
            {
                return _anonymous;
            }
            
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

            // Returns authenticated user
            return new AuthenticationState(
                new ClaimsPrincipal(
                    new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType")));
        }

        public async Task<bool> NotifyAuthentication(string token)
        {
            Task<AuthenticationState> authState;
            bool isAuthenticatedOutput;
            // Try to catch expired token
            try
            {
                await _apiHelper.GetLoggedInUserInfo(token);
                var authenticatedUser = new ClaimsPrincipal(
                            new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), 
                            "jwtAuthType"));
                authState = Task.FromResult(new AuthenticationState(authenticatedUser));
                NotifyAuthenticationStateChanged(authState);
                isAuthenticatedOutput = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);   
                authState = Task.FromResult(_anonymous);
                isAuthenticatedOutput = false;
            }
            
            return isAuthenticatedOutput;
        }

        public async Task NotifyLogout()
        {
            string tokenStorageLocationKey = _config["authTokenStorageKey"];
            await _localStorage.RemoveItemAsync(tokenStorageLocationKey);          
            var authState = Task.FromResult(_anonymous);
            _apiHelper.LogOffUser();
            _httpClient.DefaultRequestHeaders.Authorization = null;
            NotifyAuthenticationStateChanged(authState);
        }
    }
}
