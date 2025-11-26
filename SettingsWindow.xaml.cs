using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using ReminderApp.Helpers;
using ReminderApp.Models;

namespace ReminderApp
{
    public partial class SettingsWindow : Window
    {
        private readonly AppSettings _settings;
        private GlobalHotkey? _testHotkey;

        public bool SettingsChanged { get; private set; }

        public SettingsWindow(AppSettings settings)
        {
            InitializeComponent();
            _settings = settings;

            // Load current settings
            WinCheckBox.IsChecked = _settings.UseWinKey;
            CtrlCheckBox.IsChecked = _settings.UseCtrlKey;
            AltCheckBox.IsChecked = _settings.UseAltKey;
            ShiftCheckBox.IsChecked = _settings.UseShiftKey;

            // Select the current key
            foreach (ComboBoxItem item in KeyComboBox.Items)
            {
                if (item.Content.ToString() == _settings.HotKey)
                {
                    KeyComboBox.SelectedItem = item;
                    break;
                }
            }

            UpdatePreview();

            // Add change handlers
            WinCheckBox.Checked += (s, e) => UpdatePreview();
            WinCheckBox.Unchecked += (s, e) => UpdatePreview();
            CtrlCheckBox.Checked += (s, e) => UpdatePreview();
            CtrlCheckBox.Unchecked += (s, e) => UpdatePreview();
            AltCheckBox.Checked += (s, e) => UpdatePreview();
            AltCheckBox.Unchecked += (s, e) => UpdatePreview();
            ShiftCheckBox.Checked += (s, e) => UpdatePreview();
            ShiftCheckBox.Unchecked += (s, e) => UpdatePreview();

            Closing += SettingsWindow_Closing;
        }

        private void KeyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            // Don't update if controls aren't initialized yet
            if (PreviewTextBlock == null || StatusTextBlock == null)
                return;

            var preview = GetCurrentHotkeyString();
            PreviewTextBlock.Text = preview;
            StatusTextBlock.Text = "";
        }

        private string GetCurrentHotkeyString()
        {
            var parts = new System.Collections.Generic.List<string>();

            // Return default if controls aren't initialized yet
            if (WinCheckBox == null || KeyComboBox == null)
                return "Win+Shift+L";

            if (WinCheckBox.IsChecked == true) parts.Add("Win");
            if (CtrlCheckBox?.IsChecked == true) parts.Add("Ctrl");
            if (AltCheckBox?.IsChecked == true) parts.Add("Alt");
            if (ShiftCheckBox?.IsChecked == true) parts.Add("Shift");

            if (KeyComboBox.SelectedItem is ComboBoxItem item)
            {
                parts.Add(item.Content.ToString() ?? "");
            }

            return string.Join("+", parts);
        }

        private void TestHotkey_Click(object sender, RoutedEventArgs e)
        {
            // Clean up any existing test hotkey
            _testHotkey?.Dispose();
            _testHotkey = null;

            if (!ValidateSelection())
            {
                return;
            }

            var helper = new WindowInteropHelper(this);
            _testHotkey = new GlobalHotkey();

            var key = GetSelectedKey();
            var modifiers = GetModifiers();

            if (_testHotkey.Register(helper.Handle, modifiers, key))
            {
                StatusTextBlock.Text = $"✓ Success! The hotkey {GetCurrentHotkeyString()} is available and will work.";
                StatusTextBlock.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
                _testHotkey.Dispose();
                _testHotkey = null;
            }
            else
            {
                StatusTextBlock.Text = $"✗ Failed! The hotkey {GetCurrentHotkeyString()} is already in use by another application. Please choose a different combination.";
                StatusTextBlock.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
            }
        }

        private bool ValidateSelection()
        {
            // At least one modifier should be selected
            if (WinCheckBox.IsChecked != true &&
                CtrlCheckBox.IsChecked != true &&
                AltCheckBox.IsChecked != true &&
                ShiftCheckBox.IsChecked != true)
            {
                StatusTextBlock.Text = "⚠ Please select at least one modifier key (Win, Ctrl, Alt, or Shift).";
                StatusTextBlock.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Orange);
                return false;
            }

            if (KeyComboBox.SelectedItem == null)
            {
                StatusTextBlock.Text = "⚠ Please select a key.";
                StatusTextBlock.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Orange);
                return false;
            }

            return true;
        }

        private System.Windows.Input.Key GetSelectedKey()
        {
            if (KeyComboBox.SelectedItem is ComboBoxItem item)
            {
                var keyString = item.Content.ToString() ?? "L";
                if (Enum.TryParse<System.Windows.Input.Key>(keyString, true, out var key))
                {
                    return key;
                }
            }
            return System.Windows.Input.Key.L;
        }

        private uint GetModifiers()
        {
            uint modifiers = 0;
            if (WinCheckBox.IsChecked == true) modifiers |= 0x0008; // MOD_WIN
            if (CtrlCheckBox.IsChecked == true) modifiers |= 0x0002; // MOD_CONTROL
            if (AltCheckBox.IsChecked == true) modifiers |= 0x0001; // MOD_ALT
            if (ShiftCheckBox.IsChecked == true) modifiers |= 0x0004; // MOD_SHIFT
            return modifiers;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateSelection())
            {
                return;
            }

            // Update settings
            _settings.UseWinKey = WinCheckBox.IsChecked == true;
            _settings.UseCtrlKey = CtrlCheckBox.IsChecked == true;
            _settings.UseAltKey = AltCheckBox.IsChecked == true;
            _settings.UseShiftKey = ShiftCheckBox.IsChecked == true;

            if (KeyComboBox.SelectedItem is ComboBoxItem item)
            {
                _settings.HotKey = item.Content.ToString() ?? "L";
            }

            _settings.Save();
            SettingsChanged = true;

            MessageBox.Show(
                $"Hotkey saved as {_settings.GetHotkeyDisplayString()}.\n\nThe app will restart to apply the new hotkey.",
                "Settings Saved",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SettingsWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            _testHotkey?.Dispose();
            _testHotkey = null;
        }
    }
}
