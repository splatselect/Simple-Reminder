using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ReminderApp.Models;
using ReminderApp.Services;

namespace ReminderApp
{
    public partial class ActiveRemindersWindow : Window
    {
        private readonly ReminderService _reminderService;
        private bool _closing = false;

        public ActiveRemindersWindow(ReminderService reminderService)
        {
            InitializeComponent();
            _reminderService = reminderService;

            Loaded += OnLoaded;
            Closing += (s, e) => _closing = true;
            Deactivated += (s, e) => { if (!_closing) Close(); };
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            PositionOnRightSide();
            LoadReminders();
        }

        private void PositionOnRightSide()
        {
            var workArea = SystemParameters.WorkArea;
            this.Height = workArea.Height;
            this.Top = workArea.Top;
            this.Left = workArea.Right - this.Width;
        }

        private void LoadReminders()
        {
            var activeReminders = _reminderService.GetActiveReminders()
                .OrderBy(r => r.DueTime)
                .ToList();

            RemindersItemsControl.ItemsSource = activeReminders;

            int count = activeReminders.Count;

            if (count == 0)
            {
                CountTextBlock.Text = "No active reminders";
                EmptyState.Visibility = Visibility.Visible;
                CountBadge.Visibility = Visibility.Collapsed;
            }
            else
            {
                CountTextBlock.Text = $"{count} reminder{(count != 1 ? "s" : "")}";
                EmptyState.Visibility = Visibility.Collapsed;
                CountBadge.Visibility = Visibility.Visible;
                CountBadgeText.Text = count.ToString();
            }
        }

        private void Extend_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Reminder reminder)
            {
                var snoozeWindow = new SnoozeWindow();
                snoozeWindow.ShowDialog();

                if (snoozeWindow.WasSnoozed)
                {
                    _reminderService.RemoveReminder(reminder.Id);

                    DateTime newDueTime;
                    if (snoozeWindow.SnoozeDateTime.HasValue)
                    {
                        newDueTime = snoozeWindow.SnoozeDateTime.Value;
                    }
                    else
                    {
                        newDueTime = DateTime.Now.AddMinutes(snoozeWindow.SnoozeMinutes);
                    }

                    _reminderService.AddReminder(reminder.Message, newDueTime);

                    var timeString = newDueTime.Date == DateTime.Now.Date
                        ? $"{newDueTime:h:mm tt}"
                        : $"{newDueTime:MMM d} at {newDueTime:h:mm tt}";
                    var toast = new ToastNotification("Reminder Extended", $"New time: {timeString}", 2);
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

                    var toast = new ToastNotification("Reminder Dismissed", "Reminder has been removed", 2);
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
            LoadReminders();
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

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}
