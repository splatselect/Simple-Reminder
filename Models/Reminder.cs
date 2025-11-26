using System;

namespace ReminderApp.Models
{
    public class Reminder
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public DateTime DueTime { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }

        public Reminder(string message, DateTime dueTime)
        {
            Id = Guid.NewGuid();
            Message = message;
            DueTime = dueTime;
            IsCompleted = false;
            CreatedAt = DateTime.Now;
        }
    }
}
