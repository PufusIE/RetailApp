using AutoMapper;
using Caliburn.Micro;
using Microsoft.Extensions.Configuration;
using RAWPFDesktopUI.Helpers;
using RAWPFDesktopUI.Models;
using RAWPFDesktopUI.ViewModels;
using RAWPFDesktopUILibrary.Api;
using RAWPFDesktopUILibrary.Models;
using System;
using System.Collections.Generic;
using System.Configuration.Internal;
using System.IO;
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

        private IConfiguration AddConfiguration()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
#if DEBUG
            builder.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
#else
            builder.AddJsonFile("appsettings.Production.json", optional: true, reloadOnChange: true);
#endif
            return builder.Build();
        }

        //Dependency injection
        protected override void Configure()
        {
            _container.Instance(ConfigureAutoMapper());

            _container.Instance(_container)
                .PerRequest<IProductEndPoint, ProductEndPoint>()
                .PerRequest<IUserEndpoint, UserEndpoint>()
                .PerRequest<ISaleEndpoint, SaleEndpoint>();

            _container
                 .Singleton<IWindowManager, WindowManager>()    // For caliburn micro
                 .Singleton<IEventAggregator, EventAggregator>()    // For caliburn micro
                 .Singleton<IAPIHelper, APIHelper>()
                 .Singleton<ILoggedInUserModel, LoggedInUserModel>();

            _container.RegisterInstance(typeof(IConfiguration), "IConfiguration", AddConfiguration());

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
