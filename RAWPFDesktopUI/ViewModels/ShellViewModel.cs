using Caliburn.Micro;
using RAWPFDesktopUI.EventModels;
using RAWPFDesktopUILibrary.Api;
using RAWPFDesktopUILibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RAWPFDesktopUI.ViewModels
{
    //Conductor - allows to display one form on our ShellView
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEventModel>, IHandle<OwnRoleUpdateEventModel> 
    {
        private readonly IEventAggregator _events;
        private readonly ILoggedInUser _loggedInUser;
        private readonly IAPIHelper _apiHelper;

        public ShellViewModel(IEventAggregator events,
                              ILoggedInUser loggedInUser,
                              IAPIHelper apiHelper)
        {
            _events = events;
            _loggedInUser = loggedInUser;
            _apiHelper = apiHelper;
            _events.SubscribeOnUIThread(this);

            //Display Log In as a start page
            ActivateItemAsync(IoC.Get<LoginViewModel>());
        }

        public async Task HandleAsync(LogOnEventModel message, CancellationToken cancellationToken)
        {
            await ActivateItemAsync(IoC.Get<SalesViewModel>());
            NotifyOfPropertyChange(() => IsLoggedIn);
            NotifyOfPropertyChange(() => IsLoggedOut);
        }

        public void ExitApplication()
        {
            TryCloseAsync();
        }

        public async Task UserManagment()
        {
            await ActivateItemAsync(IoC.Get<UserDisplayViewModel>());
        }

        public async Task SalePage()
        {            
            await ActivateItemAsync(IoC.Get<SalesViewModel>());
        }        

        public async Task LogOut()
        {
            _loggedInUser.ResetUserModel();
            _apiHelper.LogOffUser();
            await ActivateItemAsync(IoC.Get<LoginViewModel>());
            NotifyOfPropertyChange(() => IsLoggedIn);
            NotifyOfPropertyChange(() => IsLoggedOut);
        }

        public async Task LogIn()
        {
            await ActivateItemAsync(IoC.Get<LoginViewModel>());
        }

        public async Task HandleAsync(OwnRoleUpdateEventModel message, CancellationToken cancellationToken)
        {
            await LogOut();
        }

        public bool IsLoggedIn
        {
            get
            {
                bool output = false;

                if (string.IsNullOrWhiteSpace(_loggedInUser.Token) == false)
                {
                    output = true;
                }

                return output;
            }
        }

        public bool IsLoggedOut
        {
            get
            {
                return !IsLoggedIn;
            }
        }
    }
}
