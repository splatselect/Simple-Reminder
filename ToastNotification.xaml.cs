using System;
using System.Windows;
using System.Windows.Threading;

namespace ReminderApp
{
    public partial class ToastNotification : Window
    {
        private readonly DispatcherTimer _timer;

        public ToastNotification(string title, string message, int durationSeconds = 2)
        {
            InitializeComponent();

            TitleTextBlock.Text = title;
            MessageTextBlock.Text = message;

            // Position in bottom-right corner
            var desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - Width - 20;
            Top = desktopWorkingArea.Bottom - Height - 20;

            // Auto-close timer
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(durationSeconds)
            };
            _timer.Tick += (s, e) =>
            {
                _timer.Stop();
                Close();
            };
            _timer.Start();

            // Allow clicking to close
            MouseDown += (s, e) => Close();
        }
    }
}
