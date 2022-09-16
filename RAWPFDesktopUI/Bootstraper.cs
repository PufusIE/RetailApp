using Caliburn.Micro;
using RAWPFDesktopUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RAWPFDesktopUI
{
    internal class Bootstraper : BootstrapperBase
    {
        public Bootstraper()
        {
            Initialize();
        }
        //diplaying main window and wiring up mvvm arch
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewForAsync<ShellViewModel>();
        }
    }
}
