using System;
using System.Windows;
using ReminderApp.Services;

namespace ReminderApp
{
    public partial class ReminderWindow : Window
    {
        private readonly string _message;
        private readonly ReminderService _reminderService;
        private readonly Guid _reminderId;

        public ReminderWindow(string message, ReminderService reminderService, Guid reminderId)
        {
            InitializeComponent();
            _message = message;
            _reminderService = reminderService;
            _reminderId = reminderId;
            MessageTextBlock.Text = message;

            // Allow ESC key to close
            KeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Escape)
                {
                    _reminderService.RemoveReminder(_reminderId);
                    Close();
                }
            };

            // Play a system sound
            System.Media.SystemSounds.Exclamation.Play();
        }

        private void QuickSnooze_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button button && button.Tag != null)
            {
                int minutes = int.Parse(button.Tag.ToString());

                // Remove the old reminder and add a new one
                _reminderService.RemoveReminder(_reminderId);

                DateTime dueTime = DateTime.Now.AddMinutes(minutes);
                _reminderService.AddReminder(_message, dueTime);
                Close();
            }
        }

        private void Snooze_Click(object sender, RoutedEventArgs e)
        {
            var snoozeWindow = new SnoozeWindow();
            snoozeWindow.ShowDialog();

            if (snoozeWindow.WasSnoozed)
            {
                // Remove the old reminder and add a new one
                _reminderService.RemoveReminder(_reminderId);

                DateTime dueTime;
                if (snoozeWindow.SnoozeDateTime.HasValue)
                {
                    dueTime = snoozeWindow.SnoozeDateTime.Value;
                }
                else
                {
                    dueTime = DateTime.Now.AddMinutes(snoozeWindow.SnoozeMinutes);
                }

                _reminderService.AddReminder(_message, dueTime);
                Close();
            }
        }

        private void Dismiss_Click(object sender, RoutedEventArgs e)
        {
            _reminderService.RemoveReminder(_reminderId);
            Close();
        }
    }
}
