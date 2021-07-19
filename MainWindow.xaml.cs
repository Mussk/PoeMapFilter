using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Resx = PoeMapFIlter.resources.lang.Resources;

namespace PoeMapFilter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        ObservableCollection<string> ModsToShow;

        public MainWindow()
        {           
            InitializeComponent();
        }

        public void SetDataContexts(Map map) {

            mapName_textBlock.Text = map.MapName;
            mapAtlasRegion_textBlock.Text = map.MapAtlasRegion;

            mapItemQuantity_textBlock.Text = string.Format(Resx.QuantityTextBox, map.MapItemQuantity);
            mapTier_textBlock.Text = Resx.TierTextBox + map.MapTier;

            ModsToShow = new ObservableCollection<string>(map.MapMods);

            Mods_ListBox.ItemsSource = ModsToShow;
            
        }

        public void ChangeRowBackgroundColorListBox(Brush brush, string mod)
        {
            var rows = GetListBoxRows(Mods_ListBox);

               foreach (var row in rows) 
                   if (PoeWikiJSONIteraction.ReplaceDigits(row.Content.ToString())
                    .Equals(PoeWikiJSONIteraction.ReplaceDigits(mod)))
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
                    GenerateItemContainerGeneratorListBox(listBox);

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
    }
}
