using System;
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
    }
}
