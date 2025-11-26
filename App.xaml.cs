using System;
using System.Threading;
using System.Windows;

namespace ReminderApp
{
    public partial class App : Application
    {
        private static Mutex? _mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Ensure only one instance runs
            const string mutexName = "SimpleRemindersApp_SingleInstance";
            _mutex = new Mutex(true, mutexName, out bool createdNew);

            if (!createdNew)
            {
                // Another instance is already running
                MessageBox.Show(
                    "Simple Reminders is already running. Check the system tray.",
                    "Already Running",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                Shutdown();
                return;
            }

            MainWindow = new MainWindow();
            MainWindow.Show(); // Show to trigger initialization, then it will hide itself
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _mutex?.ReleaseMutex();
            _mutex?.Dispose();
            base.OnExit(e);
        }
    }
}
