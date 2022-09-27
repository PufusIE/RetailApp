using Caliburn.Micro;
using RAWPFDesktopUI.EventModels;
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

        public ShellViewModel( IEventAggregator events, SalesViewModel SalesVM)
        {
            _events = events;
            _salesVM = SalesVM;

            _events.SubscribeOnUIThread(this);

            //Display Log In as a start page
            ActivateItemAsync(IoC.Get<LoginViewModel>());
        }

        public async Task HandleAsync(LogOnEventModel message, CancellationToken cancellationToken)
        {
            await ActivateItemAsync(_salesVM);
        }
    }
}
