using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ReminderApp.Models;
using ReminderApp.Services;

namespace ReminderApp
{
    public partial class ActiveRemindersWindow : Window
    {
        private readonly ReminderService _reminderService;

        public ActiveRemindersWindow(ReminderService reminderService)
        {
            InitializeComponent();
            _reminderService = reminderService;
            LoadReminders();
        }

        private void LoadReminders()
        {
            var activeReminders = _reminderService.GetActiveReminders()
                .OrderBy(r => r.DueTime)
                .ToList();

            RemindersItemsControl.ItemsSource = activeReminders;

            // Update count
            int count = activeReminders.Count;
            CountTextBlock.Text = count == 0
                ? "No active reminders"
                : $"{count} active reminder{(count != 1 ? "s" : "")}";
        }

        private void Extend_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Reminder reminder)
            {
                var snoozeWindow = new SnoozeWindow();
                snoozeWindow.ShowDialog();

                if (snoozeWindow.WasSnoozed)
                {
                    // Remove the old reminder
                    _reminderService.RemoveReminder(reminder.Id);

                    // Add a new one with extended time
                    var newDueTime = DateTime.Now.AddMinutes(snoozeWindow.SnoozeMinutes);
                    _reminderService.AddReminder(reminder.Message, newDueTime);

                    // Show notification
                    var toast = new ToastNotification(
                        "Reminder Extended",
                        $"New time: {newDueTime:h:mm tt}",
                        2);
                    toast.Show();

                    LoadReminders();
                }
            }
        }

        private void Dismiss_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Reminder reminder)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to dismiss this reminder?\n\n\"{reminder.Message}\"",
                    "Confirm Dismiss",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _reminderService.RemoveReminder(reminder.Id);

                    var toast = new ToastNotification(
                        "Reminder Dismissed",
                        "Reminder has been removed",
                        2);
                    toast.Show();

                    LoadReminders();
                }
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadReminders();
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            var quickNoteWindow = new QuickNoteWindow(_reminderService);
            quickNoteWindow.ShowDialog();
            LoadReminders(); // Refresh the list after creating a new reminder
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            var settings = AppSettings.Load();
            var settingsWindow = new SettingsWindow(settings);
            settingsWindow.ShowDialog();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
