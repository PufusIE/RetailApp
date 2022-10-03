using Caliburn.Micro;
using RAWPFDesktopUI.Models;
using RAWPFDesktopUILibrary.Api;
using RAWPFDesktopUILibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RAWPFDesktopUI.ViewModels
{
    public class UserDisplayViewModel : Screen
    {
        private readonly StatusInfoViewModel _status;
        private readonly IWindowManager _window;
        private readonly IUserEndpoint _userEndpoint;

        public UserDisplayViewModel(StatusInfoViewModel status, IWindowManager window, IUserEndpoint userEndpoint)
        {
            _status = status;
            _window = window;
            _userEndpoint = userEndpoint;
        }

        private BindingList<UserModel> _users;

        public BindingList<UserModel> Users
        {
            get { return _users; }
            set 
            { 
                _users = value;
                NotifyOfPropertyChange(() => Users);
            }
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            //Checking if logged in user have access to this page
            try
            {
                await LoadUsers();
            }
            catch (Exception ex)
            {
                //Setting setup for status window
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "System Error";

                var statusInfo = IoC.Get<StatusInfoViewModel>();

                if (ex.Message.ToLower() == "unauthorized")
                {
                    statusInfo.UpdateMessageInfo("Unauthorized Access", "You do not have access to Administration Form.");
                    _window.ShowDialogAsync(statusInfo, null, settings);
                }
                else
                {
                    statusInfo.UpdateMessageInfo("Fatal Exception", ex.Message);
                    _window.ShowDialogAsync(statusInfo, null, settings);
                }

                await TryCloseAsync();
            }
        }

        private async Task LoadUsers()
        {
            var userList = await _userEndpoint.GetAll();
            Users = new BindingList<UserModel>(userList);
        }
    }
}
