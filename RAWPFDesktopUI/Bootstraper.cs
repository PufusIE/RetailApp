using AutoMapper;
using Caliburn.Micro;
using RAWPFDesktopUI.Helpers;
using RAWPFDesktopUI.Models;
using RAWPFDesktopUI.ViewModels;
using RAWPFDesktopUILibrary.Api;
using RAWPFDesktopUILibrary.Helpers;
using RAWPFDesktopUILibrary.Models;
using System;
using System.Collections.Generic;
using System.Configuration.Internal;
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
        // DI container
        private SimpleContainer _container = new SimpleContainer();
        public Bootstraper()
        {
            Initialize();

            //Fix for passwordbox
            ConventionManager.AddElementConvention<PasswordBox>(
            PasswordBoxHelper.BoundPasswordProperty,
            "Password",
            "PasswordChanged");
        }

        public IMapper ConfigureAutoMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ProductModel, ProductDisplayModel>();
                cfg.CreateMap<CartItemModel, CartItemDisplayModel>();
            });

            var output = config.CreateMapper();
            return output;
        }

        //Dependency injection
        protected override void Configure()
        {
            _container.Instance(ConfigureAutoMapper());

            _container.Instance(_container)
                .PerRequest<IProductEndPoint, ProductEndPoint>()
                .PerRequest<ISaleEndpoint, SaleEndpoint>();

            _container
                 .Singleton<IWindowManager, WindowManager>()
                 .Singleton<IEventAggregator, EventAggregator>()
                 .Singleton<IAPIHelper, APIHelper>()
                 .Singleton<ILoggedInUser, LoggedInUser>()
                 .Singleton<IConfigHelper, ConfigHelper>();

            //Reflection
            GetType().Assembly.GetTypes()
                 .Where(type => type.IsClass)
                 .Where(type => type.Name.EndsWith("ViewModel"))
                 .ToList()
                 .ForEach(viewModelType => _container.RegisterPerRequest(
                     viewModelType, viewModelType.Name, viewModelType));
        }

        //Diplaying main window and wiring up mvvm arch
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
