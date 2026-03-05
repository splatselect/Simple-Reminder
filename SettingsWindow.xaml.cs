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

            // Load current settings - hotkey 1
            WinCheckBox.IsChecked = _settings.UseWinKey;
            CtrlCheckBox.IsChecked = _settings.UseCtrlKey;
            AltCheckBox.IsChecked = _settings.UseAltKey;
            ShiftCheckBox.IsChecked = _settings.UseShiftKey;

            foreach (ComboBoxItem item in KeyComboBox.Items)
            {
                if (item.Content.ToString() == _settings.HotKey)
                {
                    KeyComboBox.SelectedItem = item;
                    break;
                }
            }

            // Load current settings - hotkey 2
            WinCheckBox2.IsChecked = _settings.UseWinKey2;
            CtrlCheckBox2.IsChecked = _settings.UseCtrlKey2;
            AltCheckBox2.IsChecked = _settings.UseAltKey2;
            ShiftCheckBox2.IsChecked = _settings.UseShiftKey2;

            foreach (ComboBoxItem item in KeyComboBox2.Items)
            {
                if (item.Content.ToString() == _settings.HotKey2)
                {
                    KeyComboBox2.SelectedItem = item;
                    break;
                }
            }

            UpdatePreview();
            UpdatePreview2();

            // Change handlers - hotkey 1
            WinCheckBox.Checked += (s, e) => UpdatePreview();
            WinCheckBox.Unchecked += (s, e) => UpdatePreview();
            CtrlCheckBox.Checked += (s, e) => UpdatePreview();
            CtrlCheckBox.Unchecked += (s, e) => UpdatePreview();
            AltCheckBox.Checked += (s, e) => UpdatePreview();
            AltCheckBox.Unchecked += (s, e) => UpdatePreview();
            ShiftCheckBox.Checked += (s, e) => UpdatePreview();
            ShiftCheckBox.Unchecked += (s, e) => UpdatePreview();

            // Change handlers - hotkey 2
            WinCheckBox2.Checked += (s, e) => UpdatePreview2();
            WinCheckBox2.Unchecked += (s, e) => UpdatePreview2();
            CtrlCheckBox2.Checked += (s, e) => UpdatePreview2();
            CtrlCheckBox2.Unchecked += (s, e) => UpdatePreview2();
            AltCheckBox2.Checked += (s, e) => UpdatePreview2();
            AltCheckBox2.Unchecked += (s, e) => UpdatePreview2();
            ShiftCheckBox2.Checked += (s, e) => UpdatePreview2();
            ShiftCheckBox2.Unchecked += (s, e) => UpdatePreview2();

            Closing += SettingsWindow_Closing;
        }

        private void KeyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePreview();
        }

        private void KeyComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePreview2();
        }

        private void UpdatePreview()
        {
            if (PreviewTextBlock == null || StatusTextBlock == null)
                return;

            PreviewTextBlock.Text = GetCurrentHotkeyString();
            StatusTextBlock.Text = "";
        }

        private void UpdatePreview2()
        {
            if (PreviewTextBlock2 == null)
                return;

            PreviewTextBlock2.Text = GetCurrentHotkeyString2();
        }

        private string GetCurrentHotkeyString()
        {
            var parts = new System.Collections.Generic.List<string>();

            if (WinCheckBox == null || KeyComboBox == null)
                return "Win+Shift+L";

            if (WinCheckBox.IsChecked == true) parts.Add("Win");
            if (CtrlCheckBox?.IsChecked == true) parts.Add("Ctrl");
            if (AltCheckBox?.IsChecked == true) parts.Add("Alt");
            if (ShiftCheckBox?.IsChecked == true) parts.Add("Shift");

            if (KeyComboBox.SelectedItem is ComboBoxItem item)
                parts.Add(item.Content.ToString() ?? "");

            return string.Join("+", parts);
        }

        private string GetCurrentHotkeyString2()
        {
            var parts = new System.Collections.Generic.List<string>();

            if (WinCheckBox2 == null || KeyComboBox2 == null)
                return "Win+Shift+A";

            if (WinCheckBox2.IsChecked == true) parts.Add("Win");
            if (CtrlCheckBox2?.IsChecked == true) parts.Add("Ctrl");
            if (AltCheckBox2?.IsChecked == true) parts.Add("Alt");
            if (ShiftCheckBox2?.IsChecked == true) parts.Add("Shift");

            if (KeyComboBox2.SelectedItem is ComboBoxItem item)
                parts.Add(item.Content.ToString() ?? "");

            return string.Join("+", parts);
        }

        private void TestHotkey_Click(object sender, RoutedEventArgs e)
        {
            _testHotkey?.Dispose();
            _testHotkey = null;

            if (!ValidateSelection())
                return;

            var helper = new WindowInteropHelper(this);
            _testHotkey = new GlobalHotkey(9000);

            var key = GetSelectedKey();
            var modifiers = GetModifiers();

            if (_testHotkey.Register(helper.Handle, modifiers, key))
            {
                StatusTextBlock.Text = $"✓ Both hotkeys are available and will work.";
                StatusTextBlock.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
                _testHotkey.Dispose();
                _testHotkey = null;
            }
            else
            {
                StatusTextBlock.Text = $"✗ {GetCurrentHotkeyString()} is already in use by another application.";
                StatusTextBlock.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
            }
        }

        private bool ValidateSelection()
        {
            if (WinCheckBox.IsChecked != true &&
                CtrlCheckBox.IsChecked != true &&
                AltCheckBox.IsChecked != true &&
                ShiftCheckBox.IsChecked != true)
            {
                StatusTextBlock.Text = "⚠ Quick Note: select at least one modifier key.";
                StatusTextBlock.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Orange);
                return false;
            }

            if (KeyComboBox.SelectedItem == null)
            {
                StatusTextBlock.Text = "⚠ Quick Note: please select a key.";
                StatusTextBlock.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Orange);
                return false;
            }

            if (WinCheckBox2.IsChecked != true &&
                CtrlCheckBox2.IsChecked != true &&
                AltCheckBox2.IsChecked != true &&
                ShiftCheckBox2.IsChecked != true)
            {
                StatusTextBlock.Text = "⚠ Active Reminders: select at least one modifier key.";
                StatusTextBlock.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Orange);
                return false;
            }

            if (KeyComboBox2.SelectedItem == null)
            {
                StatusTextBlock.Text = "⚠ Active Reminders: please select a key.";
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
                    return key;
            }
            return System.Windows.Input.Key.L;
        }

        private uint GetModifiers()
        {
            uint modifiers = 0;
            if (WinCheckBox.IsChecked == true) modifiers |= 0x0008;
            if (CtrlCheckBox.IsChecked == true) modifiers |= 0x0002;
            if (AltCheckBox.IsChecked == true) modifiers |= 0x0001;
            if (ShiftCheckBox.IsChecked == true) modifiers |= 0x0004;
            return modifiers;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateSelection())
                return;

            // Save hotkey 1
            _settings.UseWinKey = WinCheckBox.IsChecked == true;
            _settings.UseCtrlKey = CtrlCheckBox.IsChecked == true;
            _settings.UseAltKey = AltCheckBox.IsChecked == true;
            _settings.UseShiftKey = ShiftCheckBox.IsChecked == true;

            if (KeyComboBox.SelectedItem is ComboBoxItem item1)
                _settings.HotKey = item1.Content.ToString() ?? "L";

            // Save hotkey 2
            _settings.UseWinKey2 = WinCheckBox2.IsChecked == true;
            _settings.UseCtrlKey2 = CtrlCheckBox2.IsChecked == true;
            _settings.UseAltKey2 = AltCheckBox2.IsChecked == true;
            _settings.UseShiftKey2 = ShiftCheckBox2.IsChecked == true;

            if (KeyComboBox2.SelectedItem is ComboBoxItem item2)
                _settings.HotKey2 = item2.Content.ToString() ?? "A";

            _settings.Save();
            SettingsChanged = true;

            MessageBox.Show(
                $"Hotkeys saved.\n\nQuick Note: {_settings.GetHotkeyDisplayString()}\nActive Reminders: {_settings.GetHotkeyDisplayString2()}\n\nThe app will restart to apply the new hotkeys.",
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
