using Caliburn.Micro;
using RAWPFDesktopUI.Helpers;
using RAWPFDesktopUILibrary.Api;
using RAWPFDesktopUILibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWPFDesktopUI.ViewModels
{
    public class LoginViewModel : Screen
    {
		private readonly IAPIHelper _apiHelper;
		private readonly ILoggedInUser _loggedInUser;
		private string _username;
		private string _password;
        private string _errorMessage;

		public LoginViewModel(IAPIHelper apiHelper, ILoggedInUser loggedInUser)
		{
			_apiHelper = apiHelper;
			_loggedInUser = loggedInUser;
		}

		//Username textbox
        public string Username
		{
			get { return _username; }
			set 
			{
				_username = value; 
				NotifyOfPropertyChange(() => Username);
                NotifyOfPropertyChange(() => CanLogIn);
            }
        }

		//Password textbox
		public string Password
		{
			get { return _password; }
			set 
			{
				_password = value; 
				NotifyOfPropertyChange(() => Password);
				NotifyOfPropertyChange(() => CanLogIn);
            }
        }

		//Error visability conditions
		public bool IsErrorVisible
        {
			get 
			{
				bool output = false;
				if (ErrorMessage?.Length >0)
				{
					output = true;
				}
				return output; 
			}
		}

		//Error message
		public string ErrorMessage
		{
			get { return _errorMessage; }
			set 
			{
				_errorMessage = value;
				NotifyOfPropertyChange(() => ErrorMessage);
				NotifyOfPropertyChange(() => IsErrorVisible);
            }
		}

		//Log In button visability conditions
		public bool CanLogIn
		{
			get
			{
				bool output = false;

				if (Username?.Length > 0 && Password?.Length > 0)
				{
					output = true;
				}
				return output;
			}
		}

		//Log In button
		public async Task LogIn()
		{
			try
			{
				ErrorMessage = "";
				// Calling /Token endpoint
				var result = await _apiHelper.Authenticate(Username, Password);

				// Calling api/user endpoint
				await _apiHelper.GetLoggedInUserInfo(result.Access_Token);				
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}
		}
	}
}
