using System;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;

namespace ReminderApp.Helpers
{
    public class GlobalHotkey : IDisposable
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int HOTKEY_ID = 9000;

        // Modifiers
        private const uint MOD_ALT = 0x0001;
        private const uint MOD_CONTROL = 0x0002;
        private const uint MOD_SHIFT = 0x0004;
        private const uint MOD_WIN = 0x0008;

        private IntPtr _windowHandle;
        private HwndSource? _source;
        public event EventHandler? HotkeyPressed;

        public bool Register(IntPtr windowHandle, uint modifiers, Key key)
        {
            _windowHandle = windowHandle;
            _source = HwndSource.FromHwnd(_windowHandle);

            if (_source != null)
            {
                _source.AddHook(HwndHook);
            }

            return RegisterHotKey(_windowHandle, HOTKEY_ID, modifiers, (uint)KeyInterop.VirtualKeyFromKey(key));
        }

        // Legacy method for backwards compatibility
        public bool Register(IntPtr windowHandle)
        {
            return Register(windowHandle, MOD_WIN | MOD_SHIFT, Key.L);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;

            if (msg == WM_HOTKEY && wParam.ToInt32() == HOTKEY_ID)
            {
                HotkeyPressed?.Invoke(this, EventArgs.Empty);
                handled = true;
            }

            return IntPtr.Zero;
        }

        public void Dispose()
        {
            if (_source != null)
            {
                _source.RemoveHook(HwndHook);
                _source = null;
            }

            UnregisterHotKey(_windowHandle, HOTKEY_ID);
        }
    }
}
