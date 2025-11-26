using System;
using System.Windows;
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

        private void MinutesSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateTimeDisplay();
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
            else
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

            // Show when the reminder will appear
            var dueTime = DateTime.Now.AddMinutes(minutes);
            ReminderTimeDisplay.Text = $"Reminder at {dueTime:h:mm tt}";
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
