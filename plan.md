For a Windows reminder app with the features you described, I'd recommend C# with WPF (Windows Presentation Foundation) as the easiest option. Here's why:

Native Windows integration
Easy global hotkey support
Simple UI creation
Built-in timer/scheduling capabilities
Can create full-screen overlays easily
Quick Implementation Plan:
1. Project Structure

Collapse
Save
Copy
1
2
3
4
5
6
7
8
9
10
11
ReminderApp/
├── App.xaml
├── MainWindow.xaml (hidden system tray app)
├── QuickNoteWindow.xaml (popup for creating reminders)
├── ReminderWindow.xaml (full-screen reminder display)
├── Models/
│   └── Reminder.cs
├── Services/
│   └── ReminderService.cs
└── Helpers/
    └── GlobalHotkey.cs
2. Core Components
A. Global Hotkey Handler

Use Windows API (user32.dll) to register global hotkey (e.g., Win+Shift+R)
When pressed, show QuickNoteWindow
B. Quick Note Window

Small popup with:
TextBox for note
DateTimePicker or quick buttons (5 min, 15 min, 1 hour, custom)
Save button
C. Reminder Service

Store reminders in a List or SQLite database
Use System.Threading.Timer or DispatcherTimer
Check every minute for due reminders
D. Full-Screen Reminder

Topmost window
Covers entire screen
Shows reminder text prominently
Dismiss button
3. Key Code Snippets
Global Hotkey Registration:

csharp

Collapse
Save
Copy
1
2
3
4
5
[DllImport("user32.dll")]
private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

// In MainWindow constructor
RegisterHotKey(handle, HOTKEY_ID, MOD_WIN | MOD_SHIFT, (uint)Keys.R);
Full-Screen Window:

csharp

Collapse
Save
Copy
1
2
3
4
5
6
7
8
public void ShowFullScreenReminder(string message)
{
    var reminderWindow = new ReminderWindow(message);
    reminderWindow.WindowState = WindowState.Maximized;
    reminderWindow.WindowStyle = WindowStyle.None;
    reminderWindow.Topmost = true;
    reminderWindow.ShowDialog();
}
4. Development Steps
Create WPF project in Visual Studio
Implement system tray icon (hide main window)
Add global hotkey functionality
Create quick note input window
Implement reminder storage and timer system
Create full-screen reminder window
Add notification sound (optional)
5. Alternative Options
If you want something even simpler:

Python + Tkinter/PyQt: Easier to code but requires Python installation
Electron + JavaScript: Web technologies, but larger app size
AutoHotkey: Super simple for basic version, limited UI capabilities
Would you like me to provide more detailed code examples for any specific component?