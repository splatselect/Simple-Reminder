using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using ReminderApp.Models;

namespace ReminderApp.Services
{
    public class ReminderService
    {
        private readonly List<Reminder> _reminders = new List<Reminder>();
        private readonly DispatcherTimer _timer;

        public event EventHandler<Reminder>? ReminderDue;

        public ReminderService()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(10) // Check every 10 seconds
            };
            _timer.Tick += CheckReminders;
            _timer.Start();
        }

        public void AddReminder(string message, DateTime dueTime)
        {
            var reminder = new Reminder(message, dueTime);
            _reminders.Add(reminder);
        }

        private void CheckReminders(object? sender, EventArgs e)
        {
            var now = DateTime.Now;
            var dueReminders = _reminders
                .Where(r => !r.IsCompleted && r.DueTime <= now)
                .ToList();

            foreach (var reminder in dueReminders)
            {
                reminder.IsCompleted = true;
                ReminderDue?.Invoke(this, reminder);
            }
        }

        public List<Reminder> GetActiveReminders()
        {
            return _reminders.Where(r => !r.IsCompleted).ToList();
        }

        public void RemoveReminder(Guid id)
        {
            _reminders.RemoveAll(r => r.Id == id);
        }
    }
}
