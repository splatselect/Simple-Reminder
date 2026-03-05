using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Input;

namespace ReminderApp.Models
{
    public class AppSettings
    {
        public bool UseWinKey { get; set; } = true;
        public bool UseShiftKey { get; set; } = true;
        public bool UseCtrlKey { get; set; } = false;
        public bool UseAltKey { get; set; } = false;
        public string HotKey { get; set; } = "L";

        // Active Reminders hotkey
        public bool UseWinKey2 { get; set; } = true;
        public bool UseShiftKey2 { get; set; } = true;
        public bool UseCtrlKey2 { get; set; } = false;
        public bool UseAltKey2 { get; set; } = false;
        public string HotKey2 { get; set; } = "A";

        public List<Reminder> SavedReminders { get; set; } = new List<Reminder>();

        private static string SettingsPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SimpleReminders",
            "settings.json");

        public static AppSettings Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    var json = File.ReadAllText(SettingsPath);
                    return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
            }
            catch
            {
                // If load fails, return default settings
            }

            return new AppSettings();
        }

        public void Save()
        {
            try
            {
                var directory = Path.GetDirectoryName(SettingsPath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsPath, json);
            }
            catch
            {
                // Silently fail if save fails
            }
        }

        public string GetHotkeyDisplayString()
        {
            var parts = new System.Collections.Generic.List<string>();
            if (UseWinKey) parts.Add("Win");
            if (UseCtrlKey) parts.Add("Ctrl");
            if (UseAltKey) parts.Add("Alt");
            if (UseShiftKey) parts.Add("Shift");
            parts.Add(HotKey);
            return string.Join("+", parts);
        }

        public Key GetKey()
        {
            if (Enum.TryParse<Key>(HotKey, true, out var key))
            {
                return key;
            }
            return Key.L; // Default fallback
        }

        public string GetHotkeyDisplayString2()
        {
            var parts = new System.Collections.Generic.List<string>();
            if (UseWinKey2) parts.Add("Win");
            if (UseCtrlKey2) parts.Add("Ctrl");
            if (UseAltKey2) parts.Add("Alt");
            if (UseShiftKey2) parts.Add("Shift");
            parts.Add(HotKey2);
            return string.Join("+", parts);
        }

        public Key GetKey2()
        {
            if (Enum.TryParse<Key>(HotKey2, true, out var key))
            {
                return key;
            }
            return Key.A; // Default fallback
        }
    }
}
