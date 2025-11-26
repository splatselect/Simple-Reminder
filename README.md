# Simple Reminders

A lightweight Windows reminder application built with C# and WPF.

## Features

- **Customizable Global Hotkey**: Default `Win+Shift+L` - press anywhere to create a reminder
- **System Tray**: Runs quietly in the background with custom icon
- **Quick Time Selection**: Slider to set reminders from 1 to 120 minutes
- **Full-Screen Alerts**: Reminders appear as full-screen notifications
- **Snooze Functionality**: Reschedule reminders for 1-60 minutes
- **Active Reminders Management**: View, extend, or dismiss all pending reminders
- **Toast Notifications**: Auto-dismissing confirmations when reminders are created
- **Customizable Hotkey**: Test and configure your own global hotkey combination
- **Single-File Executable**: No installation required - just run the .exe

## Quick Start

**For Users**: Simply run `Simple Reminder.exe` from the `publish` folder - no installation needed!

**For Developers**:
1. Build the project:
   ```
   dotnet build
   ```

2. Create single-file executable:
   ```
   dotnet publish -c Release -o publish
   ```

3. Run from:
   ```
   publish\Simple Reminder.exe
   ```

## Usage

1. **Launch the app**: It will minimize to the system tray (look for the custom icon in the bottom-right corner)

2. **Create a reminder**:
   - Press `Win+Shift+L` (or your custom hotkey), OR
   - Double-click the system tray icon, OR
   - Right-click the system tray icon and select "New Reminder"

3. **Set your reminder**:
   - Type your reminder message
   - Use the slider to choose time (1-120 minutes)
   - A toast notification will confirm your reminder is set

4. **When a reminder appears**:
   - It will take over your entire screen
   - Click "Snooze" to reschedule (1-60 minutes), OR
   - Click "Dismiss" or press `ESC` to close it

5. **Manage active reminders**:
   - Right-click the system tray icon and select "View Active Reminders"
   - See all pending reminders with due times
   - Extend or dismiss any reminder

6. **Customize hotkey**:
   - Right-click the system tray icon and select "Settings"
   - Choose modifier keys (Win, Ctrl, Alt, Shift) and a letter
   - Test the hotkey before saving to ensure it's not in use

## Requirements

- Windows 10/11
- .NET 8.0 or later (included in published single-file executable)

## Project Structure

```
ReminderApp/
├── App.xaml                      # Application entry point with single-instance check
├── MainWindow.xaml               # Hidden main window with system tray
├── QuickNoteWindow.xaml          # Popup for creating reminders
├── ReminderWindow.xaml           # Full-screen reminder display
├── SnoozeWindow.xaml             # Snooze time selection
├── SettingsWindow.xaml           # Hotkey customization with testing
├── ActiveRemindersWindow.xaml    # View and manage all pending reminders
├── ToastNotification.xaml        # Auto-dismissing notification popup
├── icon.ico                      # Custom application and tray icon
├── Models/
│   ├── Reminder.cs               # Reminder data model
│   └── AppSettings.cs            # User preferences (saved to %APPDATA%)
├── Services/
│   └── ReminderService.cs        # Timer and reminder management
└── Helpers/
    └── GlobalHotkey.cs           # Windows global hotkey registration (Win32 API)
```

## Notes

- The app checks for due reminders every 10 seconds
- Reminders are stored in memory (not persisted between sessions)
- Settings are saved to `%APPDATA%\SimpleReminders\settings.json`
- Only one instance of the app can run at a time (enforced by Mutex)
- The icon is embedded as a resource in the single-file executable
- All toast notifications auto-dismiss after 2 seconds
- Completely vibe coded. Don't @ me