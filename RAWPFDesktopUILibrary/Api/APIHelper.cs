using Microsoft.Extensions.Configuration;
using RAWPFDesktopUILibrary.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RAWPFDesktopUILibrary.Api
{
    public class APIHelper : IAPIHelper
    {
        private HttpClient _apiClient;
        private readonly ILoggedInUserModel _loggedInUser;
        private readonly IConfiguration _config;

        public HttpClient ApiClient
        {
            get { return _apiClient; }            
        }

        public APIHelper(HttpClient apiC, ILoggedInUserModel loggedInUser, IConfiguration config)
        {
            _apiClient = apiC;
            _loggedInUser = loggedInUser;
            _config = config;
            InitializeClient();
        }

        private void InitializeClient()
        {
            //pulling server from app.config
            string api = _config.GetValue<string>("api");

            //configuring our API Client
            _apiClient = new HttpClient
            {
                BaseAddress = new Uri(api)
            };
            _apiClient.DefaultRequestHeaders.Accept.Clear();
            _apiClient.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        //Calling /Token endpoint
        public async Task<AuthenticatedUser> Authenticate(string username, string password)
        {
            var data = new TokenUserModel { Username = username, Password = password, Grant_Type = "g" };           
                        
            using (HttpResponseMessage response = await _apiClient.PostAsJsonAsync("/Token", data))
            {
                //Reading response from api call and populating AuthenticatedUser model
                if (response.IsSuccessStatusCode == true)
                {
                    var result = await response.Content.ReadAsAsync<AuthenticatedUser>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public void LogOffUser()
        {
            _apiClient.DefaultRequestHeaders.Clear();
        }

        //Calling api/user endpoint and mapping model
        public async Task GetLoggedInUserInfo(string token)
        {
            _apiClient.DefaultRequestHeaders.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Clear();
            //Passing bearer token with every call
            _apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            _apiClient.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (HttpResponseMessage response = await _apiClient.GetAsync("/api/User"))
            {
                //reading respons from api call and populating AuthenticatedUser model
                if (response.IsSuccessStatusCode == true)
                {
                    var result = await response.Content.ReadAsAsync<LoggedInUserModel>();
                    //Could be swapped with automapper in the future
                    _loggedInUser.Token = token;
                    _loggedInUser.CreateDate = result.CreateDate;
                    _loggedInUser.EmailAddress = result.EmailAddress;
                    _loggedInUser.FirstName = result.FirstName;
                    _loggedInUser.LastName = result.LastName;
                    _loggedInUser.Id = result.Id;                    
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
