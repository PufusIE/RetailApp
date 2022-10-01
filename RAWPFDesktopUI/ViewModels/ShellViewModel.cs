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
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEventModel>
    {
        private readonly IEventAggregator _events;
        private readonly SalesViewModel _salesVM;
        private readonly ILoggedInUser _loggedInUser;
        private readonly IAPIHelper _apiHelper;

        public ShellViewModel(IEventAggregator events, SalesViewModel SalesVM, ILoggedInUser loggedInUser, IAPIHelper apiHelper)
        {
            _events = events;
            _salesVM = SalesVM;
            _loggedInUser = loggedInUser;
            _apiHelper = apiHelper;
            _events.SubscribeOnUIThread(this);

            //Display Log In as a start page
            ActivateItemAsync(IoC.Get<LoginViewModel>());
        }

        public async Task HandleAsync(LogOnEventModel message, CancellationToken cancellationToken)
        {
            await ActivateItemAsync(_salesVM);
            NotifyOfPropertyChange(() => IsLoggedIn);
        }

        public void ExitApplication()
        {
            TryCloseAsync();
        }

        public void LogOut()
        {
            _loggedInUser.ResetUserModel();
            _apiHelper.LogOffUser();
            ActivateItemAsync(IoC.Get<LoginViewModel>());
            NotifyOfPropertyChange(() => IsLoggedIn);
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
    }
}
