# Simple Reminders

A lightweight Windows reminder application built with C# and WPF.

## Features

- **Global Hotkey**: Press `Win+Shift+L` anywhere to create a reminder
- **System Tray**: Runs quietly in the background, accessible from the system tray
- **Quick Time Selection**: Create reminders with preset times (5 min, 15 min, 30 min, 1 hour, 2 hours, tomorrow)
- **Custom Date**: Set reminders for specific dates
- **Full-Screen Alerts**: Reminders appear as full-screen notifications with sound

## How to Run

1. Build the project:
   ```
   dotnet build
   ```

2. Run the application:
   ```
   dotnet run
   ```

   Or run the executable from:
   ```
   bin\Debug\net8.0-windows\ReminderApp.exe
   ```

## Usage

1. **Launch the app**: It will minimize to the system tray (look for the icon in the bottom-right corner)

2. **Create a reminder**:
   - Press `Win+Shift+L` (global hotkey), OR
   - Double-click the system tray icon, OR
   - Right-click the system tray icon and select "New Reminder"

3. **Set your reminder**:
   - Type your reminder message
   - Choose a time (quick buttons or custom date)
   - The reminder will appear at the scheduled time

4. **When a reminder appears**:
   - It will take over your entire screen
   - You'll hear a notification sound
   - Click "Dismiss" or press `ESC` to close it

## Requirements

- Windows 10/11
- .NET 8.0 or later

## Project Structure

```
ReminderApp/
├── App.xaml                    # Application entry point
├── MainWindow.xaml             # Hidden main window with system tray
├── QuickNoteWindow.xaml        # Popup for creating reminders
├── ReminderWindow.xaml         # Full-screen reminder display
├── Models/
│   └── Reminder.cs             # Reminder data model
├── Services/
│   └── ReminderService.cs      # Timer and reminder management
└── Helpers/
    └── GlobalHotkey.cs         # Windows global hotkey registration
```

## Notes

- The app checks for due reminders every 10 seconds
- Reminders are stored in memory (not persisted between sessions)
- Only one instance of the global hotkey can be registered at a time
