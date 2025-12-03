using System;
using System.Windows;
using System.Windows.Input;

namespace ReminderApp
{
    public partial class SnoozeWindow : Window
    {
        public int SnoozeMinutes { get; private set; }
        public bool WasSnoozed { get; private set; }

        public SnoozeWindow()
        {
            InitializeComponent();
            UpdateTimeDisplay();
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

            // Show when the snoozed reminder will appear
            var dueTime = DateTime.Now.AddMinutes(minutes);
            var timeString = dueTime.Date == DateTime.Now.Date
                ? $"Today at {dueTime:h:mm tt}"
                : $"{dueTime:MMM d} at {dueTime:h:mm tt}";
            ReminderTimeDisplay.Text = $"Snooze until: {timeString}";
        }

        private void Snooze_Click(object sender, RoutedEventArgs e)
        {
            SnoozeMinutes = (int)MinutesSlider.Value;
            WasSnoozed = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            WasSnoozed = false;
            Close();
        }
    }
}
