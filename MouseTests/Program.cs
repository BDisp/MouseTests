using System.Runtime.InteropServices;

class Program
{
    // Define constants
    private const uint ENABLE_MOUSE_INPUT = 0x0010;
    private const uint ENABLE_EXTENDED_FLAGS = 0x0080;
    private const uint STD_INPUT_HANDLE = unchecked((uint)-10);

    // Define input event types
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
        public MOUSE_EVENT_RECORD MouseEvent;
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

        // Set the console mode to enable mouse input
        SetConsoleMode(hConsoleInput, ENABLE_MOUSE_INPUT | ENABLE_EXTENDED_FLAGS);

        INPUT_RECORD[] records = new INPUT_RECORD[1];
        uint numRead;

        Console.WriteLine("Mouse events capture started. Press Ctrl+C to exit.");

        // Handle Ctrl+C to exit
        Console.CancelKeyPress += (sender, e) =>
        {
            Console.WriteLine("\nExiting...");
            Environment.Exit(0);
        };

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
                    if (currentLine > 0)
                    {
                        Console.SetCursorPosition(0, currentLine - 1);
                    }
                    else
                    {
                        Console.SetCursorPosition(0, currentLine); // Stay on the current line
                    }

                    // Clear the current line
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, currentLine > 0 ? currentLine - 1 : currentLine);

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
            }
        }
    }
}
