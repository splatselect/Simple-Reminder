using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using ReminderApp.Helpers;
using ReminderApp.Models;
using ReminderApp.Services;

namespace ReminderApp
{
    public partial class MainWindow : Window
    {
        private NotifyIcon? _notifyIcon;
        private GlobalHotkey? _globalHotkey;
        private GlobalHotkey? _activeRemindersHotkey;
        private ActiveRemindersWindow? _activeRemindersWindow;
        private readonly ReminderService _reminderService;
        private readonly AppSettings _settings;

        public MainWindow()
        {
            InitializeComponent();

            _settings = AppSettings.Load();
            _reminderService = new ReminderService(_settings);
            _reminderService.ReminderDue += OnReminderDue;

            InitializeSystemTray();
            Closing += MainWindow_Closing;

            // Use Loaded event which fires when window is ready
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MainWindow_Loaded; // Only run once
            RegisterHotkey();
            Hide(); // Hide the window after hotkey is registered
        }

        private void InitializeSystemTray()
        {
            var hotkeyString = _settings.GetHotkeyDisplayString();

            // Load custom icon from embedded resource
            System.Drawing.Icon trayIcon;
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var resourceName = "ReminderApp.icon.ico";
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        trayIcon = new System.Drawing.Icon(stream);
                    }
                    else
                    {
                        trayIcon = SystemIcons.Information;
                    }
                }
            }
            catch
            {
                trayIcon = SystemIcons.Information;
            }

            _notifyIcon = new NotifyIcon
            {
                Icon = trayIcon,
                Visible = true,
                Text = $"Simple Reminders - {hotkeyString} to create reminder"
            };

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add($"New Reminder ({hotkeyString})", null, (s, e) => ShowQuickNoteWindow());
            contextMenu.Items.Add("View Active Reminders", null, (s, e) => ShowActiveRemindersWindow());
            contextMenu.Items.Add("-");
            contextMenu.Items.Add("Settings...", null, (s, e) => ShowSettingsWindow());
            contextMenu.Items.Add("-");
            contextMenu.Items.Add("Exit", null, (s, e) => System.Windows.Application.Current.Shutdown());

            _notifyIcon.ContextMenuStrip = contextMenu;
            _notifyIcon.DoubleClick += (s, e) => ShowActiveRemindersWindow();
        }

        private void MainWindow_SourceInitialized(object? sender, EventArgs e)
        {
            RegisterHotkey();
        }

        private void RegisterHotkey()
        {
            // Clean up existing hotkeys if any
            _globalHotkey?.Dispose();
            _activeRemindersHotkey?.Dispose();

            var helper = new WindowInteropHelper(this);

            // Register quick note hotkey (ID 9000)
            _globalHotkey = new GlobalHotkey(9000);
            var modifiers = GetModifiersFromSettings();
            var key = _settings.GetKey();
            var hotkeyString = _settings.GetHotkeyDisplayString();

            if (_globalHotkey.Register(helper.Handle, modifiers, key))
            {
                _globalHotkey.HotkeyPressed += (s, e) => ShowQuickNoteWindow();
            }
            else
            {
                var result = System.Windows.MessageBox.Show(
                    $"Failed to register global hotkey {hotkeyString}. " +
                    "Another application may be using it.\n\n" +
                    "Would you like to open Settings to choose a different hotkey?",
                    "Hotkey Registration Failed",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    ShowSettingsWindow();
                }
            }

            // Register active reminders hotkey (ID 9001)
            _activeRemindersHotkey = new GlobalHotkey(9001);
            var modifiers2 = GetModifiers2FromSettings();
            var key2 = _settings.GetKey2();

            if (_activeRemindersHotkey.Register(helper.Handle, modifiers2, key2))
            {
                _activeRemindersHotkey.HotkeyPressed += (s, e) => ToggleActiveRemindersWindow();
            }
        }

        private uint GetModifiersFromSettings()
        {
            uint modifiers = 0;
            if (_settings.UseWinKey) modifiers |= 0x0008;    // MOD_WIN
            if (_settings.UseCtrlKey) modifiers |= 0x0002;   // MOD_CONTROL
            if (_settings.UseAltKey) modifiers |= 0x0001;    // MOD_ALT
            if (_settings.UseShiftKey) modifiers |= 0x0004;  // MOD_SHIFT
            return modifiers;
        }

        private uint GetModifiers2FromSettings()
        {
            uint modifiers = 0;
            if (_settings.UseWinKey2) modifiers |= 0x0008;
            if (_settings.UseCtrlKey2) modifiers |= 0x0002;
            if (_settings.UseAltKey2) modifiers |= 0x0001;
            if (_settings.UseShiftKey2) modifiers |= 0x0004;
            return modifiers;
        }

        private void ShowQuickNoteWindow()
        {
            var quickNoteWindow = new QuickNoteWindow(_reminderService);
            quickNoteWindow.Show();
            quickNoteWindow.Activate();
        }

        private void ShowActiveRemindersWindow()
        {
            if (_activeRemindersWindow != null && _activeRemindersWindow.IsLoaded)
            {
                _activeRemindersWindow.Activate();
                return;
            }

            _activeRemindersWindow = new ActiveRemindersWindow(_reminderService);
            _activeRemindersWindow.Closed += (s, e) => _activeRemindersWindow = null;
            _activeRemindersWindow.Show();
            _activeRemindersWindow.Activate();
        }

        private void ToggleActiveRemindersWindow()
        {
            if (_activeRemindersWindow != null && _activeRemindersWindow.IsLoaded)
            {
                _activeRemindersWindow.Close();
                _activeRemindersWindow = null;
            }
            else
            {
                ShowActiveRemindersWindow();
            }
        }

        private void ShowSettingsWindow()
        {
            var settingsWindow = new SettingsWindow(_settings);
            settingsWindow.ShowDialog();

            if (settingsWindow.SettingsChanged)
            {
                // Restart the application
                var result = System.Windows.MessageBox.Show(
                    "The application needs to restart to apply the new hotkey. Restart now?",
                    "Restart Required",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    RestartApplication();
                }
            }
        }

        private void RestartApplication()
        {
            var exePath = Process.GetCurrentProcess().MainModule?.FileName;
            if (exePath != null)
            {
                Process.Start(exePath);
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void OnReminderDue(object? sender, Models.Reminder reminder)
        {
            var reminderWindow = new ReminderWindow(reminder.Message, _reminderService, reminder.Id);
            reminderWindow.Show();
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            _globalHotkey?.Dispose();
            _activeRemindersHotkey?.Dispose();

            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
            }
        }
    }
}
