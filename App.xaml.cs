using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WK.Libraries.HotkeyListenerNS;
using System.Threading;
using System.Globalization;

namespace PoeBadMapsMod
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private MainWindow mainWindow;

        private Program program;

        private KeysManager keysManager;

        private void Application_Startup(object sender, StartupEventArgs e)
        {

            SetLanguage(Settings.LangIndex);

            mainWindow = new();

            mainWindow.Show();

            program = new(mainWindow);

            keysManager = new(program);
            keysManager.hotkey = Serialization<Hotkey>.Deserialize<Hotkey>("Hotkey.json");
            if (keysManager.hotkey.ToString().Equals("None"))
                keysManager.hotkey = KeysManager.DefaultHotkey;

            MyNotifyIcon myNotifyIcon = new(program, keysManager);

        }

        private void SetLanguage(int LangIndex) 
        {
            switch (LangIndex)
            {
                case 1:
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("ru-RU");
                    break;
                default:
                    Settings.LangIndex = Settings.DefaultLangIndex;
                    break;
            }
            
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
          


        }
    }
}
