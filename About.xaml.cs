using System.Reflection;
using System.Windows;
using System.Diagnostics;

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
            Process.Start("https://github.com/Mussk/PoeMapFilter");
        }

        private void Label_Bug_Click(object sender, RoutedEventArgs e)
        { 
            Process.Start("https://github.com/Mussk/PoeMapFilter/issues");
        }

        private void Label_Font_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.exljbris.com/fontin.html");
        }
    }
}
