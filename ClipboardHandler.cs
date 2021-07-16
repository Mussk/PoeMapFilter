using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
