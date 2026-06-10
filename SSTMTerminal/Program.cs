using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SSTMTerminal
{
    public static class Program
    {
        #region DllImport

        [DllImport("user32.dll", CharSet = CharSet.Auto, BestFitMapping = false)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto, BestFitMapping = false)]
        private static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo", SetLastError = true)]
        private static extern bool SystemParametersInfoSet(uint action, uint uiParam, uint vparam, uint init);

        #endregion

        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Process currentProcess = Process.GetCurrentProcess();

            bool allowMultipleInstances = string.Equals(ConfigurationManager.AppSettings[Constants.ALLOW_MULTIPLE_INSTANCES],
                true.ToString(), StringComparison.OrdinalIgnoreCase);

            if (!allowMultipleInstances)
            {
                var runningProcess = (from process in Process.GetProcesses()
                                      where
                                        process.Id != currentProcess.Id &&
                                        process.ProcessName.Equals(
                                          currentProcess.ProcessName,
                                          StringComparison.Ordinal)
                                      select process).FirstOrDefault();

                if (runningProcess != null)
                {
                    // SW_SHOW: Activates the window and displays it in its current size and position.
                    int SW_SHOW = 5;

                    ShowWindow(runningProcess.MainWindowHandle, SW_SHOW);
                    SetForegroundWindow(runningProcess.MainWindowHandle);

                    return;
                }
            }

            const bool IS_RESTART_WHEN_CRASH = false;

            // Set the SystemParameters.MenuDropAlignment to false, false means the aliment is left.
            SystemParametersInfoSet(0x001C, 0, 0, 0);

            do
            {
                dynamic manager = null;

                try
                {
                    manager = StartApplicationDomain(AppDomain.CurrentDomain);
                    manager.Startup();
                    break;
                }
                catch (Exception ex)
                {
                    Shutdown(manager);
                }
            } while (IS_RESTART_WHEN_CRASH);
        }

        private static dynamic StartApplicationDomain(AppDomain current)
        {
            var application = AppDomain.CreateDomain("Application", null, new AppDomainSetup
            {
                ConfigurationFile = current.SetupInformation.ConfigurationFile,
                LoaderOptimization = LoaderOptimization.SingleDomain
            });

            return application.CreateInstance(typeof(Program).Assembly.GetName().Name, typeof(AppDomainManager).FullName).Unwrap();
        }

        private static void Shutdown(dynamic manager)
        {
            if (null == manager)
            {
                return;
            }

            try
            {
                manager.Shutdown();
            }
            catch (Exception)
            {
                // When shuting down the target AppDomain by using Unload it self,
                // there might be some exceptions, there is nothing can do here, ignore the exceptions.
            }
        }
    }
}
