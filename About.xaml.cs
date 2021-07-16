using System.Reflection;
using System.Windows;
using System.Diagnostics;
using PoeMapFIlter;

namespace PoeMapFilter
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();

            About_Version_Label.Content += Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        private void Image_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe",Links.GitHubPage);
        }

        private void Label_Bug_Click(object sender, RoutedEventArgs e)
        { 
            Process.Start("explorer.exe",Links.BugReport);
        }

        private void Label_Font_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe",Links.FontLink);
        }
    }
}
