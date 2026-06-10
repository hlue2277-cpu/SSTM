using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SSTMTerminal
{
    public class AppDomainManager : MarshalByRefObject
    {
        private Assembly _entryAssembly;

        public AppDomainManager()
        {
            //
            // Since this class will be called as a separated domain without Main() method,
            // there will be no entry assembly in this domain (thus 'Assembly.GetEntryAssembly()' will not work in this whole application).
            //
            _entryAssembly = Assembly.LoadFrom(Assembly.GetCallingAssembly().CodeBase);
        }

        public void Startup()
        {
            InternalStartup(_entryAssembly);
        }

        public void Shutdown()
        {
            Application.Current.Shutdown();
            Dispatcher.CurrentDispatcher.InvokeShutdown();

            // Unload the current domain.
            AppDomain.Unload(AppDomain.CurrentDomain);
        }

        private void InternalStartup(Assembly assembly)
        {
            //
            // Shows the splash screen,
            // the splash screen will close it self when main window is loaded.
            //
            //(new SplashScreen()).Show();

            var applicationType = assembly
                .GetTypes()
                .SingleOrDefault(x => typeof(Application).IsAssignableFrom(x));

            var application = Activator.CreateInstance(applicationType) as Application;

            application.Run();
        }
    }
}
