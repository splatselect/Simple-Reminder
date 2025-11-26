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

        // Parameterless constructor for JSON serialization
        public Reminder()
        {
            Id = Guid.NewGuid();
            Message = string.Empty;
            DueTime = DateTime.Now;
            IsCompleted = false;
            CreatedAt = DateTime.Now;
        }

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
