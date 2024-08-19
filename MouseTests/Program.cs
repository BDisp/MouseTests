using System;
using System.Runtime.InteropServices;

class Program
{
    // Define constants
    private const uint ENABLE_PROCESSED_INPUT = 0x0001;
    private const uint ENABLE_MOUSE_INPUT = 0x0010;
    private const uint ENABLE_EXTENDED_FLAGS = 0x0080;
    private const uint STD_INPUT_HANDLE = unchecked((uint)-10);

    // Define input event types
    private const ushort KEY_EVENT = 0x0001;
    private const ushort MOUSE_EVENT = 0x0002;

    // Define mouse button state constants
    private const uint FROM_LEFT_1ST_BUTTON_PRESSED = 0x0001;
    private const uint RIGHTMOST_BUTTON_PRESSED = 0x0002;

    // Input record structure
    [StructLayout(LayoutKind.Explicit)]
    struct INPUT_RECORD
    {
        [FieldOffset(0)]
        public ushort EventType;
        [FieldOffset(4)]
        public KEY_EVENT_RECORD KeyEvent;
        [FieldOffset(4)]
        public MOUSE_EVENT_RECORD MouseEvent;
    }

    // Key event record structure
    [StructLayout(LayoutKind.Sequential)]
    struct KEY_EVENT_RECORD
    {
        public bool bKeyDown;
        public ushort wRepeatCount;
        public ushort wVirtualKeyCode;
        public ushort wVirtualScanCode;
        public char UnicodeChar;
        public uint dwControlKeyState;
    }

    // Mouse event record structure
    [StructLayout(LayoutKind.Sequential)]
    struct MOUSE_EVENT_RECORD
    {
        public COORD dwMousePosition;
        public uint dwButtonState;
        public uint dwControlKeyState;
        public uint dwEventFlags;
    }

    // Coordinate structure
    [StructLayout(LayoutKind.Sequential)]
    struct COORD
    {
        public short X;
        public short Y;
    }

    // DLL imports
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr GetStdHandle(uint nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool ReadConsoleInputW(IntPtr hConsoleInput, [Out] INPUT_RECORD[] lpBuffer, uint nLength, out uint lpNumberOfEventsRead);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    static void Main(string[] args)
    {
        IntPtr hConsoleInput = GetStdHandle(STD_INPUT_HANDLE);

        // Set the console mode to enable mouse and keyboard input
        SetConsoleMode(hConsoleInput, ENABLE_MOUSE_INPUT | ENABLE_EXTENDED_FLAGS | ENABLE_PROCESSED_INPUT);

        INPUT_RECORD[] records = new INPUT_RECORD[1];
        uint numRead;

        Console.WriteLine("Mouse events capture started. Press ESC to exit.");

        while (true)
        {
            // Read the input records from the console
            if (ReadConsoleInputW(hConsoleInput, records, 1, out numRead))
            {
                if (records[0].EventType == MOUSE_EVENT)
                {
                    var mouseEvent = records[0].MouseEvent;

                    // Safely move the cursor to the previous line if possible
                    int currentLine = Console.CursorTop;
                    if (currentLine > 1)
                    {
                        Console.SetCursorPosition(0, 1);
                    }
                    else
                    {
                        Console.SetCursorPosition(0, currentLine); // Stay on the current line
                    }

                    // Clear the current line
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, currentLine > 1 ? 1 : currentLine);

                    // Handle mouse button presses
                    if (mouseEvent.dwButtonState == FROM_LEFT_1ST_BUTTON_PRESSED)
                    {
                        Console.WriteLine($"Left button pressed at ({mouseEvent.dwMousePosition.X}, {mouseEvent.dwMousePosition.Y})");
                    }
                    else if (mouseEvent.dwButtonState == RIGHTMOST_BUTTON_PRESSED)
                    {
                        Console.WriteLine($"Right button pressed at ({mouseEvent.dwMousePosition.X}, {mouseEvent.dwMousePosition.Y})");
                    }
                    else if (mouseEvent.dwEventFlags == 0)
                    {
                        Console.WriteLine($"Mouse moved to ({mouseEvent.dwMousePosition.X}, {mouseEvent.dwMousePosition.Y})");
                    }
                }
                else if (records[0].EventType == KEY_EVENT)
                {
                    var keyEvent = records[0].KeyEvent;

                    if (keyEvent.bKeyDown)
                    {
                        //Console.WriteLine($"Key pressed: {keyEvent.UnicodeChar} (Virtual Key Code: {keyEvent.wVirtualKeyCode})");

                        // Example: Exit on ESC key press
                        if (keyEvent.wVirtualScanCode == 0x1B) // ESC key
                        {
                            Console.WriteLine("ESC key pressed. Exiting...");
                            break;
                        }
                    }
                }
            }
        }
    }
}
