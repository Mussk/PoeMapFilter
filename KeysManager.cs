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
        private HotkeyListener HkListener { get; set; }

        public Hotkey Hotkey { get; set; }

        public static readonly Hotkey DefaultHotkey = new(Keys.Control, Keys.B);
        
        private readonly InputSimulator sim;

        private MapWindowLogic MapWindowLogic;

        public KeysManager(MapWindowLogic MapWindowLogic)
        {
            this.MapWindowLogic = MapWindowLogic;
            this.HkListener = new();
            this.Hotkey = Serialization<Hotkey>.Deserialize<Hotkey>("Hotkey.json");
            if (this.Hotkey.ToString().Equals("None"))
                this.Hotkey = DefaultHotkey;
            this.HkListener.Add(Hotkey);

            HkListener.HotkeyPressed += Hkl_HotkeyPressed;

            sim = new InputSimulator();
        }

        private void Hkl_HotkeyPressed(object sender, HotkeyEventArgs e)
        {         
            if (e.Hotkey == Hotkey)
            {           
                sim.Keyboard.ModifiedKeyStroke(WindowsInput.Native.VirtualKeyCode.CONTROL,
                                                WindowsInput.Native.VirtualKeyCode.VK_C);

                MapWindowLogic.Run_MapWindowLogic();   
            }
        }

        public void SetNewHotkey(Hotkey hotkeyToSet) 
        {
            HkListener.Remove(Hotkey);
            if (!hotkeyToSet.ToString().Equals("None"))
                HkListener.Add(hotkeyToSet);
            else HkListener.Add(DefaultHotkey);
            Hotkey = hotkeyToSet;

        }
    }
}
