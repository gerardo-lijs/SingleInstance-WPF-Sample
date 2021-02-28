using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SingleInstance_WPF
{
    public partial class App : Application
    {
        // Single instance identifier
        private const string ApplicationSingleInstanceGuid = "{910e8c27-ab31-4043-9c5d-1382707e6c93}";
        private static System.Threading.Mutex _singleInstanceMutex;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Create mutex
            _singleInstanceMutex = new System.Threading.Mutex(true, ApplicationSingleInstanceGuid);
            var mutexIsAcquired = _singleInstanceMutex.WaitOne(TimeSpan.Zero, true);

            // TODO: Read from settings if you want your user to be able to opt-in to multiple instance or not
            var settings_AllowMultipleInstances = false;
            if (settings_AllowMultipleInstances || mutexIsAcquired)
            {
                // Show main window
                StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);

                // Release mutex
                _singleInstanceMutex.ReleaseMutex();
            }
            else
            {
                // Bring the already running application into the foreground
                SingleInstance.PostMessage((IntPtr)SingleInstance.HWND_BROADCAST, SingleInstance.WM_SHOWME, IntPtr.Zero, IntPtr.Zero);
                Shutdown();
            }
        }
    }
}
