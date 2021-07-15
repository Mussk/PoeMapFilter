using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.ObjectModel;
using PoeBadMapsMod.ViewModels;
using System.IO;
using System.Windows.Input;
using WK.Libraries.HotkeyListenerNS;
using System.Resources;
using Resx = PoeBadMapsMod.resources.lang.Resources;
using System.Threading;

namespace PoeBadMapsMod
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {

        public ObservableCollection<string> allModsToShow;

        public SelectedModViewModel selectedModViewModel;

        private List<string> allModsList;

        public static bool IsSoundSignalEnabled;

        /* 0 = English
         * 1 = Russian
        */
        public static int DefaultLangIndex = 0;

        public static int LangIndex = Serialization<int>.
                Deserialize<int>("SelectedLang.json");

        private Program program;

        private KeysManager km;

        public Settings(KeysManager km, Program program)
        {
            var poeWikiJsonIteraction = new PoeWikiJSONIteraction();

            if (poeWikiJsonIteraction.CheckConnectionWithAPI())
            {
                allModsList = poeWikiJsonIteraction.ParseJsonMods
                    (poeWikiJsonIteraction.GetJsonFromUrl(PoeWikiJSONIteraction.URL));

                InitializeComponent();

                this.program = program;

                this.km = km;

                selectedModViewModel = Serialization<SelectedModViewModel>.
                    Deserialize<SelectedModViewModel>("SelectedModViewModel.json", Thread.CurrentThread.CurrentUICulture);

                Settings_SelectedMods_ListView.DataContext = selectedModViewModel;
                Settings_Hotkey_TextBox.DataContext = km;
                Settings_Language_ComboBox.DataContext = LangIndex;

                allModsToShow = new ObservableCollection<string>(allModsList);
                Settings_AllMods_ListView.ItemsSource = allModsToShow;

                var hks = new HotkeySelectorWPF();
                hks.Enable(Settings_Hotkey_TextBox, km.hotkey);

                IsSoundSignalEnabled = Serialization<bool>.Deserialize<bool>("SoundSignal.json");
                Settings_SoundSignal_CheckBox.IsChecked = IsSoundSignalEnabled;

                //remove mods from allModsList which are already in selected mods
                RemoveSelectedMods(allModsList, selectedModViewModel.selectedModsToShow);

                this.Show();
            }
            else
            {
                MessageBox.Show(Resx.NoInternetMessage, Resx.ErrorWindowName,
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            selectedModViewModel.selectedModsToShow.Add(new SelectedModViewItem(Settings_AllMods_ListView.SelectedItem.ToString()
                                                                                , SelectedModViewItem.DefaultColor));

            allModsToShow.Remove(Settings_AllMods_ListView.SelectedItem.ToString());
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            SelectedModViewItem[] array = new SelectedModViewItem[Settings_SelectedMods_ListView.SelectedItems.Count];

            Settings_SelectedMods_ListView.SelectedItems.CopyTo(array, 0);

            for (int i = 0; i < array.Length; i++)
            {
                selectedModViewModel.selectedModsToShow.Remove(array[i]);

                allModsToShow.Add(array[i].modContext);
            }
        }

        private void Settings_AllMods_Filter_TextChanged(object sender, TextChangedEventArgs e)
        {
            var filtered = allModsList.Where(contact => contact.Contains(Settings_AllMods_Filter.Text,
                                                                                    StringComparison.InvariantCultureIgnoreCase));
            Remove_NonMatching(filtered);
            AddBack_Mods(filtered);

        }

        private void Remove_NonMatching(IEnumerable<string> filteredData)
        {
            if (!Settings_AllMods_Filter.Text.Equals(Resx.SettingsAllModsSearchPlaceholder))
            {
                for (int i = allModsToShow.Count - 1; i >= 0; i--)
                {
                    var item = allModsToShow[i];

                    if (!filteredData.Contains(item))
                    {
                        allModsToShow.Remove(item);
                    }
                }
            }
        }

        private void RemoveSelectedMods(List<string> AllModsList, ObservableCollection<SelectedModViewItem> SelectedModsList) {

            if (SelectedModsList.Count != 0)
                foreach (var mod in SelectedModsList) 
                    if (AllModsList.Contains(mod.modContext))
                        AllModsList.Remove(mod.modContext);
        }

        private void AddBack_Mods(IEnumerable<string> filteredData)
        {
            foreach (var item in filteredData)
                // If item in filtered list is not currently in ListView's source collection, add it back in
                if (!allModsToShow.Contains(item))
                    allModsToShow.Add(item);

        }

        private void Settings_SoundSignal_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            IsSoundSignalEnabled = true;
        }

        private void Settings_SoundSignal_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            IsSoundSignalEnabled = false;
        }

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {

        }

        private void Settings_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            km.setNewHotkey(new Hotkey(Settings_Hotkey_TextBox.Text));

            SerializeSettings();
        }

        private void Button_About_Click(object sender, RoutedEventArgs e)
        {
            new About().Show();
        }

        private void Settings_Language_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LangIndex = Settings_Language_ComboBox.SelectedIndex;
        }

        private void SerializeSettings() 
        {
            Serialization<SelectedModViewModel>.Serialize(selectedModViewModel, "SelectedModViewModel.json",
                                                            Thread.CurrentThread.CurrentUICulture);
            program._selectedModViewModel = Serialization<SelectedModViewModel>
                                                    .Deserialize<SelectedModViewModel>("SelectedModViewModel.json",
                                                                                       Thread.CurrentThread.CurrentUICulture);

            Serialization<Hotkey>.Serialize(km.hotkey, "Hotkey.json");
            km.hotkey = Serialization<Hotkey>.Deserialize<Hotkey>("Hotkey.json");

            Serialization<bool>.Serialize(IsSoundSignalEnabled, "SoundSignal.json");
            IsSoundSignalEnabled = Serialization<bool>.Deserialize<bool>("SoundSignal.json");

            Serialization<int>.Serialize(LangIndex, "SelectedLang.json");
            
        }

        private void Settings_Hotkey_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Settings_AllMods_Filter_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Settings_AllMods_Filter.Text == Resx.SettingsAllModsSearchPlaceholder)
            {
                Settings_AllMods_Filter.Text = "";
            }
        }

        private void Settings_AllMods_Filter_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Settings_AllMods_Filter.Text))
                Settings_AllMods_Filter.Text = Resx.SettingsAllModsSearchPlaceholder;
        }
    }
}