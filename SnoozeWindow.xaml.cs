using System;
using System.Windows;

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

        private void UpdateTimeDisplay()
        {
            if (TimeDisplay == null)
                return;

            int minutes = (int)MinutesSlider.Value;
            TimeDisplay.Text = $"{minutes} minute{(minutes != 1 ? "s" : "")}";
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
