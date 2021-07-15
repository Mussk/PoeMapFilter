using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Threading;
using System.Resources;
using System.Reflection;
using System.Globalization;
using Resx = PoeBadMapsMod.resources.lang.Resources;

namespace PoeBadMapsMod
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        ObservableCollection<string> mods_to_show;

        public MainWindow()
        {           
            InitializeComponent();
        }

        public void Set_Data_Contexts(Map map) {

            mapName_textBlock.Text = map.mapName;
            mapAtlasRegion_textBlock.Text = map.mapAtlasRegion;

            mapItemQuantity_textBlock.Text = string.Format(Resx.QuantityTextBox, map.mapItemQuantity);
            mapTier_textBlock.Text = Resx.TierTextBox + map.mapTier;

            mods_to_show = new ObservableCollection<string>(map.mapMods);

            Mods_ListBox.ItemsSource = mods_to_show;
            
        }

        public void changeRowBackgroundColorListBox(Brush brush, string mod)
        {

            var rows = GetListBoxRows(Mods_ListBox);

               foreach (var row in rows) 
                   if (PoeWikiJSONIteraction.Replace_digits(row.Content.ToString())
                    .Equals(PoeWikiJSONIteraction.Replace_digits(mod)))
                        row.Background = brush;
                   
               Mods_ListBox.UpdateLayout(); 
        }

        public IEnumerable<ListBoxItem> GetListBoxRows(ListBox listBox)
        {
            var itemsSource = listBox.ItemsSource as IEnumerable;
            if (itemsSource == null) yield return null;
            foreach (var item in itemsSource)
            {
                if (listBox.ItemContainerGenerator.Status == GeneratorStatus.NotStarted)
                {
                    GenerateItemContainerGeneratorListBox(listBox);
                }
                listBox.ScrollIntoView(item);
                var row = listBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                if (row != null) yield return row;
            }
        }

        public void GenerateItemContainerGeneratorListBox(ListBox listBox)
        {

            IItemContainerGenerator generator = listBox.ItemContainerGenerator;
            GeneratorPosition position = generator.GeneratorPositionFromIndex(0);
            using (generator.StartAt(position, GeneratorDirection.Forward, true))
            {
                foreach (object o in listBox.Items)
                {
                    DependencyObject dp = generator.GenerateNext();
                    generator.PrepareItemContainer(dp);
                }
            }

        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void Mods_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
