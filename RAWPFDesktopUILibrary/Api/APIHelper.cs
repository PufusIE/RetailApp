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
        private HttpClient apiClient;
        private readonly ILoggedInUser _loggedInUser;

        public APIHelper(HttpClient apiC, ILoggedInUser loggedInUser)
        {
            apiClient = apiC;
            _loggedInUser = loggedInUser;
            InitializeClient();
        }

        private void InitializeClient()
        {
            //pulling server from app.config
            string api = ConfigurationManager.AppSettings["api"];

            //configuring our API Client
            apiClient = new HttpClient();
            apiClient.BaseAddress = new Uri(api);
            apiClient.DefaultRequestHeaders.Accept.Clear();
            apiClient.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        //Calling /Token endpoint
        public async Task<AuthenticatedUser> Authenticate(string username, string password)
        {
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            });
                        
            using (HttpResponseMessage response = await apiClient.PostAsync("/Token", data))
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

        //Calling api/user endpoint and mapping model
        public async Task GetLoggedInUserInfo(string token)
        {
            apiClient.DefaultRequestHeaders.Clear();
            apiClient.DefaultRequestHeaders.Accept.Clear();
            //Passing bearer token with every call
            apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            apiClient.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (HttpResponseMessage response = await apiClient.GetAsync("/api/User"))
            {
                //reading respons from api call and populating AuthenticatedUser model
                if (response.IsSuccessStatusCode == true)
                {
                    var result = await response.Content.ReadAsAsync<LoggedInUser>();
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
