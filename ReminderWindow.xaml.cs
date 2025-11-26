using System;
using System.Windows;
using ReminderApp.Services;

namespace ReminderApp
{
    public partial class ReminderWindow : Window
    {
        private readonly string _message;
        private readonly ReminderService _reminderService;

        public ReminderWindow(string message, ReminderService reminderService)
        {
            InitializeComponent();
            _message = message;
            _reminderService = reminderService;
            MessageTextBlock.Text = message;

            // Allow ESC key to close
            KeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Escape)
                {
                    Close();
                }
            };

            // Play a system sound
            System.Media.SystemSounds.Exclamation.Play();
        }

        private void Snooze_Click(object sender, RoutedEventArgs e)
        {
            var snoozeWindow = new SnoozeWindow();
            snoozeWindow.ShowDialog();

            if (snoozeWindow.WasSnoozed)
            {
                var dueTime = DateTime.Now.AddMinutes(snoozeWindow.SnoozeMinutes);
                _reminderService.AddReminder(_message, dueTime);
                Close();
            }
        }

        private void Dismiss_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
