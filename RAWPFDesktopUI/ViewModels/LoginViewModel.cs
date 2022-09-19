﻿using Caliburn.Micro;
using RAWPFDesktopUI.Helpers;
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
		private string _username;
		private string _password;
        private string _errorMessage;

        public LoginViewModel(IAPIHelper apiHelper)
		{
			_apiHelper = apiHelper;
		}

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

		public async Task LogIn()
		{
			try
			{
				ErrorMessage = "";
				var result = await _apiHelper.Authenticate(Username, Password);
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}
		}
	}
}
