using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WK.Libraries.HotkeyListenerNS;
using System.Windows.Input;
using System.Windows.Forms;
using System.Windows;
using WindowsInput;

namespace PoeMapFilter
{
    public class KeysManager
    {
        private HotkeyListener hkl { get; set; }

        public Hotkey hotkey { get; set; }

        public static Hotkey DefaultHotkey = new(Keys.Control, Keys.B);
        
        private InputSimulator sim;

        public Program program;

        public KeysManager(Program program)
        {
            this.program = program;
            this.hkl = new();
            this.hotkey = DefaultHotkey;
            this.hkl.Add(hotkey);

            hkl.HotkeyPressed += Hkl_HotkeyPressed;

            sim = new InputSimulator();
        }

        private void Hkl_HotkeyPressed(object sender, HotkeyEventArgs e)
        {         
            if (e.Hotkey == hotkey)
            {           
                sim.Keyboard.ModifiedKeyStroke(WindowsInput.Native.VirtualKeyCode.CONTROL, WindowsInput.Native.VirtualKeyCode.VK_C);

                program.run_program();   
            }
        }

        public void setNewHotkey(Hotkey hotkeyToSet) 
        {
            hkl.Remove(hotkey);
            if (!hotkeyToSet.ToString().Equals("None"))
                hkl.Add(hotkeyToSet);
            else hkl.Add(DefaultHotkey);
            hotkey = hotkeyToSet;

        }
    }
}
