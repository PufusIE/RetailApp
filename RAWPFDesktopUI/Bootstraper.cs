using Caliburn.Micro;
using RAWPFDesktopUI.Helpers;
using RAWPFDesktopUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RAWPFDesktopUI
{
    internal class Bootstraper : BootstrapperBase
    {
        private SimpleContainer _container = new SimpleContainer();
        public Bootstraper()
        {
            Initialize();

            //fix for passwordbox
            ConventionManager.AddElementConvention<PasswordBox>(
            PasswordBoxHelper.BoundPasswordProperty,
            "Password",
            "PasswordChanged");
        }

        //dependency injection
        protected override void Configure()
        {
            _container.Instance(_container);

            _container
                 .Singleton<IWindowManager, WindowManager>()
                 .Singleton<IEventAggregator, EventAggregator>();

            //reflection
            GetType().Assembly.GetTypes()
                 .Where(type => type.IsClass)
                 .Where(type => type.Name.EndsWith("ViewModel"))
                 .ToList()
                 .ForEach(viewModelType => _container.RegisterPerRequest(
                     viewModelType, viewModelType.Name, viewModelType));
        }

        //diplaying main window and wiring up mvvm arch
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewForAsync<ShellViewModel>();
        }
        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }
        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }
        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}
