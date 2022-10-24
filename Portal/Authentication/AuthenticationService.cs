﻿using Blazored.LocalStorage;
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

        public async Task<AuthenticatedUserModel> LoginAsync(AuthenticationUserModel userForAuthentication)
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
            // Reading response (token)
            var authContent = await authResult.Content.ReadAsStringAsync();

            if (authResult.IsSuccessStatusCode == false)
            {
                return null;
            }

            // Converting response from json to our model
            var result = JsonSerializer.Deserialize<AuthenticatedUserModel>(authContent);

            // Cashing token to user's machine 
            await _localStorage.SetItemAsync("authToken", result.access_Token);

            // Casting authstate provider from microsoft with own child class and changing the status of the user to be logged in
            ((AuthStateProvider)_authProvider).NotifyAuthentication(result.access_Token);

            // Sends this token with each future api request 
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.access_Token);

            return result;
        }

        public async Task LogoutAsync()
        {
            // Clearing headers 
            _httpClient.DefaultRequestHeaders.Authorization = null;
            // Removing token from cash
            await _localStorage.RemoveItemAsync("authToken");
            // Notifying logout even
            ((AuthStateProvider)_authProvider).NotifyLogout();
        }
    }
}