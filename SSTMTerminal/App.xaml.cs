using Autofac;
using SSTMTerminal.Controls;
using Genesis;
using Genesis.Events;
using Genesis.Exceptions;
using Genesis.Logging;
using Genesis.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SSTMTerminal
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private IExceptionHandler _exceptionHandler;

        public App()
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);
            CVR_Reader.CVR_Reader.Close(out var result);
		}

		protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var bootstrapper = new Bootstrapper();
            var currentShell = Path.GetFileNameWithoutExtension(this.GetType().Assembly.Location);
            var files = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            var features = (new List<string>() { currentShell }).Concat(from item in files
                                                                        where FeatureFilter(item)
                                                                        select Path.GetFileNameWithoutExtension(item)).ToArray();

            bootstrapper.Initialize((builder) =>
            {
                builder.RegisterType<Shell>();
                builder.RegisterType<CustomizedExceptionHandler>().As<IExceptionHandler>().SingleInstance().OwnedByLifetimeScope().OnActivated((o)=>
                {
                    o.Instance.Container = bootstrapper.Scope;
                    o.Instance.Logger = o.Context.Resolve<ILogger>();
                });
                builder.RegisterType<CustomizedInterceptor>().As<IServiceProxyInterceptor>().SingleInstance();
                builder.RegisterType<Loader>().As<ILoader>().SingleInstance().OwnedByLifetimeScope().OnActivated((o) =>
                {
                    o.Instance.Container = bootstrapper.Scope;
                    o.Instance.EventAggregator = o.Context.Resolve<IEventAggregator>();
                    o.Instance.Logger = o.Context.Resolve<ILogger>();
                    o.Instance.ZoneManager = o.Context.Resolve<IZoneManager>();
                });
            }, features);

            bootstrapper.Run(
                scope =>
                {
                    var shell = scope.Resolve<Shell>();
                    _exceptionHandler = scope.Resolve<IExceptionHandler>();
                    LoadStyles();
                    LoadDataTemplates();
                    MainWindow = shell;
                    MainWindow.ContentRendered += (sender, args) =>
                    {
                        scope.Resolve<IEventAggregator>().GetEvent<StartupEvent>().Publish(null);
                    };
					MainWindow.Closing += (sender, args) =>
                    {
                        scope.Resolve<IEventAggregator>().GetEvent<ClosingEvent>().Publish(null);
                    }; 
                    MainWindow.Show();
                    return shell;
                },
                scope =>
                {
                    var loader = scope.Resolve<ILoader>();
                    loader.Initialize();
                });
        }

		private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = _exceptionHandler.HandleException(sender, e.Exception);
        }

        private void LoadStyles()
        {
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("/SSTMTerminal;component/Styles/Style.xaml", UriKind.RelativeOrAbsolute)
            });
        }

        private void LoadDataTemplates()
        {
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("/SSTMTerminal;component/DataTemplates/DataTemplate.xaml", UriKind.RelativeOrAbsolute)
            });
        }

        private bool FeatureFilter(string item)
        {
            var extension = Path.GetExtension(item);
            var name = Path.GetFileNameWithoutExtension(item);
            
            //过滤掉非feature的dll文件
            var result = (extension.Equals(".dll", StringComparison.OrdinalIgnoreCase)
                && !name.StartsWith(typeof(Bootstrapper).Assembly.GetName().Name, StringComparison.OrdinalIgnoreCase)
                && !name.StartsWith("autofac", StringComparison.OrdinalIgnoreCase)
                && !name.StartsWith("log4net", StringComparison.OrdinalIgnoreCase)
                && !name.StartsWith("system", StringComparison.OrdinalIgnoreCase)
                && !name.StartsWith("microsoft", StringComparison.OrdinalIgnoreCase)
                && !name.StartsWith("WltRS", StringComparison.OrdinalIgnoreCase)
                && !name.StartsWith("CVR_Reader", StringComparison.OrdinalIgnoreCase)
                && !name.StartsWith("sdtapi", StringComparison.OrdinalIgnoreCase)
                && !name.StartsWith("termb", StringComparison.OrdinalIgnoreCase)
                && !name.StartsWith("TransitionEffects", StringComparison.OrdinalIgnoreCase)
                && !name.StartsWith("ThoughtWorks.QRCode", StringComparison.OrdinalIgnoreCase)
                && !name.StartsWith("ServiceStack", StringComparison.OrdinalIgnoreCase)
                && !name.StartsWith("Printer", StringComparison.OrdinalIgnoreCase)
                && !name.StartsWith("bitmap", StringComparison.OrdinalIgnoreCase)
                && !name.StartsWith("BPLADLL", StringComparison.OrdinalIgnoreCase)
                && !name.StartsWith("ByUsbInt", StringComparison.OrdinalIgnoreCase)
                && !name.StartsWith("ByPortAPI", StringComparison.OrdinalIgnoreCase)
                && !name.StartsWith("LabelUSBPrintDll", StringComparison.OrdinalIgnoreCase)
                && !name.StartsWith("PortOperationLib", StringComparison.OrdinalIgnoreCase)
                && !name.StartsWith("SimpleLogModule", StringComparison.OrdinalIgnoreCase)
                && !name.StartsWith("USBPrintDll", StringComparison.OrdinalIgnoreCase)
                && !name.StartsWith("TKIOSKDLL", StringComparison.OrdinalIgnoreCase)
                && !name.StartsWith("PrintCtrl", StringComparison.OrdinalIgnoreCase)
                && !name.StartsWith("WindowsInput", StringComparison.OrdinalIgnoreCase)
                && !name.StartsWith("Newtonsoft.Json", StringComparison.OrdinalIgnoreCase)
                && !name.StartsWith("Castle.Core", StringComparison.OrdinalIgnoreCase)
                && !name.StartsWith("CuCustomWndAPI", StringComparison.OrdinalIgnoreCase));

            return result;
        }
    }
}