using System.Windows;
using WK.Libraries.HotkeyListenerNS;
using System.Threading;
using System.Globalization;

namespace PoeMapFilter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainWindow mainWindow;

        private MapWindowLogic MapWindowLogic;

        private KeysManager keysManager;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SetLanguage(Settings.LangIndex);

            mainWindow = new();

            mainWindow.Show();

            MapWindowLogic = new(mainWindow);

            keysManager = new(MapWindowLogic);
            
            new MyNotifyIcon(MapWindowLogic, keysManager);

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
    }
}
