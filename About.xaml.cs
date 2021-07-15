using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace PoeBadMapsMod
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

        }

        private void Label_Bug_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Label_Font_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
