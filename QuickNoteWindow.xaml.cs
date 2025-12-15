using System;
using System.Windows;
using System.Windows.Input;
using ReminderApp.Services;

namespace ReminderApp
{
    public partial class QuickNoteWindow : Window
    {
        private readonly ReminderService _reminderService;
        private DateTime? _customDateTime;

        public QuickNoteWindow(ReminderService reminderService)
        {
            InitializeComponent();
            _reminderService = reminderService;

            InitializeCalendarPickers();
            MessageTextBox.Focus();
            UpdateTimeDisplay();
        }

        private void InitializeCalendarPickers()
        {
            // Populate hours (1-12)
            for (int i = 1; i <= 12; i++)
            {
                HourPicker.Items.Add(i);
            }

            // Populate minutes (00-59)
            for (int i = 0; i < 60; i++)
            {
                MinutePicker.Items.Add(i.ToString("00"));
            }

            // Set defaults to current time
            var now = DateTime.Now;
            HourPicker.SelectedItem = now.Hour % 12 == 0 ? 12 : now.Hour % 12;
            MinutePicker.SelectedItem = now.Minute.ToString("00");
            AmPmPicker.SelectedIndex = now.Hour >= 12 ? 1 : 0;
            DatePicker.SelectedDate = DateTime.Today;
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

                // Show calendar button when slider extends
                if (CalendarButton.Visibility == Visibility.Collapsed)
                {
                    CalendarButton.Visibility = Visibility.Visible;
                }
            }
        }

        private void CalendarButton_Click(object sender, RoutedEventArgs e)
        {
            // Toggle calendar panel visibility
            if (CalendarPanel.Visibility == Visibility.Visible)
            {
                CalendarPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                CalendarPanel.Visibility = Visibility.Visible;
            }
        }

        private void SetDateTime_Click(object sender, RoutedEventArgs e)
        {
            if (DatePicker.SelectedDate == null || HourPicker.SelectedItem == null ||
                MinutePicker.SelectedItem == null || AmPmPicker.SelectedItem == null)
            {
                MessageBox.Show("Please select a complete date and time.", "Incomplete Selection",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var date = DatePicker.SelectedDate.Value;
            var hour = (int)HourPicker.SelectedItem;
            var minute = int.Parse(MinutePicker.SelectedItem!.ToString()!);
            var isPM = (AmPmPicker.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString() == "PM";

            // Convert to 24-hour format
            if (isPM && hour != 12) hour += 12;
            if (!isPM && hour == 12) hour = 0;

            _customDateTime = new DateTime(date.Year, date.Month, date.Day, hour, minute, 0);

            if (_customDateTime <= DateTime.Now)
            {
                MessageBox.Show("Please select a future date and time.", "Invalid Time",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                _customDateTime = null;
                return;
            }

            // Hide calendar panel and update display
            CalendarPanel.Visibility = Visibility.Collapsed;

            // Update the display to show selected date/time
            var timeString = _customDateTime.Value.Date == DateTime.Now.Date
                ? $"Today at {_customDateTime.Value:h:mm tt}"
                : $"{_customDateTime.Value:MMM d} at {_customDateTime.Value:h:mm tt}";
            ReminderTimeDisplay.Text = $"📅 Selected: {timeString}";
            TimeDisplay.Text = "Custom Date/Time";
        }

        private void UpdateTimeDisplay()
        {
            if (TimeDisplay == null || ReminderTimeDisplay == null)
                return;

            // Don't update if custom datetime is set
            if (_customDateTime.HasValue)
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

            // If calendar has selections but user didn't click "Set Date/Time", auto-commit them
            if (!_customDateTime.HasValue &&
                DatePicker.SelectedDate != null &&
                HourPicker.SelectedItem != null &&
                MinutePicker.SelectedItem != null &&
                AmPmPicker.SelectedItem != null)
            {
                var date = DatePicker.SelectedDate.Value;
                var hour = (int)HourPicker.SelectedItem;
                var minute = int.Parse(MinutePicker.SelectedItem!.ToString()!);
                var isPM = (AmPmPicker.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString() == "PM";

                // Convert to 24-hour format
                if (isPM && hour != 12) hour += 12;
                if (!isPM && hour == 12) hour = 0;

                var selectedDateTime = new DateTime(date.Year, date.Month, date.Day, hour, minute, 0);

                if (selectedDateTime > DateTime.Now)
                {
                    _customDateTime = selectedDateTime;
                }
            }

            DateTime dueTime;
            string toastMessage;

            if (_customDateTime.HasValue)
            {
                // Use custom date/time from calendar
                dueTime = _customDateTime.Value;
                var timeString = dueTime.Date == DateTime.Now.Date
                    ? $"{dueTime:h:mm tt}"
                    : $"{dueTime:MMM d} at {dueTime:h:mm tt}";
                toastMessage = $"Reminder set for {timeString}";
            }
            else
            {
                // Use slider value
                int minutes = (int)MinutesSlider.Value;
                dueTime = DateTime.Now.AddMinutes(minutes);
                toastMessage = $"Reminder set for {dueTime:h:mm tt} ({minutes} minute{(minutes != 1 ? "s" : "")})";
            }

            _reminderService.AddReminder(message, dueTime);

            // Show auto-closing toast notification
            var toast = new ToastNotification("Timer Started!", toastMessage, 2);
            toast.Show();

            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
