using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PoeMapFilter
{
    public class ClipboardHandler
    {

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetOpenClipboardWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        public static Process GetProcessLockingClipboard()
        {
            int processId;
            GetWindowThreadProcessId(GetOpenClipboardWindow(), out processId);

            return Process.GetProcessById(processId);
        }
    }
}
