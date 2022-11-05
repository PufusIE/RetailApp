using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Portal.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Portal.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authProvider;
    private readonly ILocalStorageService _localStorage;
    private readonly IConfiguration _config;
    private readonly string _tokenStorageLocationKey;

    public AuthenticationService(HttpClient httpClient, AuthenticationStateProvider authProvider, ILocalStorageService localStorage, IConfiguration config)
    {
        _httpClient = httpClient;
        _authProvider = authProvider;
        _localStorage = localStorage;
        _config = config;
        _tokenStorageLocationKey = _config["authTokenStorageKey"];
    }

    public async Task<AuthenticatedUserModel> LoginAsync(AuthenticationUserModel userForAuthentication)
    {
        var data = new TokenUserModel
        {
            Username = userForAuthentication.Email,
            Password = userForAuthentication.Password,
            Grant_Type = "password"
        };

        var tokenEndpoint = _config["api"] + _config["tokenEndpoint"];
        var authResult = await _httpClient.PostAsJsonAsync(tokenEndpoint, data);        
        var authContent = await authResult.Content.ReadAsStringAsync();

        if (authResult.IsSuccessStatusCode == false)
        {
            return null;
        }

        // Converting response from json to our model
        var result = JsonSerializer.Deserialize<AuthenticatedUserModel>(authContent);

        // Cashing token to user's machine 
        await _localStorage.SetItemAsync(_tokenStorageLocationKey, result.access_Token);

        // Casting authstate provider from microsoft with own child class and changing the status of the user to be logged in
        await ((AuthStateProvider)_authProvider).NotifyAuthentication(result.access_Token);
                

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.access_Token);

        return result;
    }

    public async Task LogoutAsync()
    {    
        await ((AuthStateProvider)_authProvider).NotifyLogout();
    }
}