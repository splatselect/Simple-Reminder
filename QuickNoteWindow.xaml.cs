using System;
using System.Windows;
using System.Windows.Input;
using ReminderApp.Services;

namespace ReminderApp
{
    public partial class QuickNoteWindow : Window
    {
        private readonly ReminderService _reminderService;

        public QuickNoteWindow(ReminderService reminderService)
        {
            InitializeComponent();
            _reminderService = reminderService;

            MessageTextBox.Focus();
            UpdateTimeDisplay();
        }

        private void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true; // Prevent the Enter key from adding a new line
                StartTimer_Click(sender, e);
            }
        }

        private void MinutesSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateTimeDisplay();
        }

        private void MinutesSlider_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // If slider is at or near maximum when released, extend it by 5 hours (300 minutes)
            if (MinutesSlider.Value >= MinutesSlider.Maximum - 1)
            {
                MinutesSlider.Maximum += 300; // Add 5 more hours
            }
        }

        private void UpdateTimeDisplay()
        {
            if (TimeDisplay == null || ReminderTimeDisplay == null)
                return;

            int minutes = (int)MinutesSlider.Value;

            // Format display text
            if (minutes < 60)
            {
                TimeDisplay.Text = $"{minutes} minute{(minutes != 1 ? "s" : "")}";
            }
            else if (minutes < 1440) // Less than 24 hours
            {
                int hours = minutes / 60;
                int remainingMinutes = minutes % 60;
                if (remainingMinutes == 0)
                {
                    TimeDisplay.Text = $"{hours} hour{(hours != 1 ? "s" : "")}";
                }
                else
                {
                    TimeDisplay.Text = $"{hours}h {remainingMinutes}m";
                }
            }
            else // 24 hours or more
            {
                int days = minutes / 1440;
                int remainingHours = (minutes % 1440) / 60;
                int remainingMinutes = minutes % 60;

                if (remainingHours == 0 && remainingMinutes == 0)
                {
                    TimeDisplay.Text = $"{days} day{(days != 1 ? "s" : "")}";
                }
                else if (remainingMinutes == 0)
                {
                    TimeDisplay.Text = $"{days}d {remainingHours}h";
                }
                else
                {
                    TimeDisplay.Text = $"{days}d {remainingHours}h {remainingMinutes}m";
                }
            }

            // Show when the reminder will appear
            var dueTime = DateTime.Now.AddMinutes(minutes);
            var timeString = dueTime.Date == DateTime.Now.Date
                ? $"Today at {dueTime:h:mm tt}"
                : $"{dueTime:MMM d} at {dueTime:h:mm tt}";
            ReminderTimeDisplay.Text = $"Reminder: {timeString}";
        }

        private void StartTimer_Click(object sender, RoutedEventArgs e)
        {
            var message = MessageTextBox.Text.Trim();

            if (string.IsNullOrEmpty(message))
            {
                MessageBox.Show(
                    "Please enter a reminder message.",
                    "Empty Message",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            int minutes = (int)MinutesSlider.Value;
            var dueTime = DateTime.Now.AddMinutes(minutes);

            _reminderService.AddReminder(message, dueTime);

            // Show auto-closing toast notification
            var toast = new ToastNotification(
                "Timer Started!",
                $"Reminder set for {dueTime:h:mm tt} ({minutes} minute{(minutes != 1 ? "s" : "")})",
                2);
            toast.Show();

            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
