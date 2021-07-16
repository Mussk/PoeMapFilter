using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using PoeMapFilter.ViewModels;
using System.Collections.ObjectModel;
using System.Media;
using Resx = PoeMapFIlter.resources.lang.Resources;

namespace PoeMapFilter
{
    public class Program
    {
        public Map map { get; set; }

        private MainWindow window;

        public SelectedModViewModel _selectedModViewModel { get; set; }

        public Program(MainWindow window) {

            this.window = window;

            map = new("Initial_map_name", 0, "Initial_map_atlas_region", 0);

            window.Set_Data_Contexts(map);
  
            _selectedModViewModel = Serialization<SelectedModViewModel>.
                Deserialize<SelectedModViewModel>("SelectedModViewModel.json");

        }

        public string getClipboardData()
        {

            string data = "";

            while (isClipboardLocked()) { }
            
            if (Clipboard.ContainsText())            
                data = Clipboard.GetData(DataFormats.Text).ToString();
            else
                window.Visibility = Visibility.Hidden;

            return data;

        }
        public bool parseClipboard()
        {
            int offsetCounter = 0;

            var clipboard_raw_data = getClipboardData();

            string[] map_by_lines = clipboard_raw_data.Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            if (!map_by_lines[0].Equals(Resx.ItemClassString)) return false;

            string mapName = map_by_lines.ElementAtOrDefault(3);

            int mapTier = Int32.Parse(map_by_lines.ElementAtOrDefault(5).Split(' ')[2]);

            string mapAtlasRegion;

            if (this.map.checkImplicitMod(map_by_lines))
            {
                offsetCounter++;
                mapAtlasRegion = map_by_lines.ElementAtOrDefault(13).Split('(')[0].Trim();
            }
            else
            {
                mapAtlasRegion = map_by_lines.ElementAtOrDefault(6).Split(':')[1].Trim();
            }
            double mapItemQuantity = parseMapQuantity(map_by_lines.ElementAtOrDefault(7-offsetCounter));

            List<string> mapMods = parseMapMods(map_by_lines);

            this.map.mapName = mapName;
            this.map.mapTier = mapTier;
            this.map.mapAtlasRegion = mapAtlasRegion;
            this.map.mapItemQuantity = Math.Round(mapItemQuantity * 100, 0);
            this.map.mapMods = mapMods;

            return true;
        }

        public double parseMapQuantity(string raw_data) {

            int pFrom = raw_data.IndexOf("+");
            //blue maps???
            int pTo = raw_data.LastIndexOf("% (augmented)");

            return Double.Parse(raw_data.Substring(pFrom, pTo - pFrom)) / 100;

        }

        public List<string> parseMapMods(string[] map_by_lines)
        {
            string current_line = "";

            int index_line = map_by_lines.Length - 3;

            List<string> mods = new();

            while (!current_line.Equals(Map.MAP_SEPARATOR)) {

                mods.Add(map_by_lines[index_line]);

                index_line--;

                current_line = map_by_lines[index_line];

            }

            mods.Reverse();

            return mods;
        }

        public void Check_mods(ObservableCollection<SelectedModViewItem> selectedMods, List<string> map_mods) {

            foreach (var selectedMod in selectedMods)
                if (map_mods.Select(e => PoeWikiJSONIteraction.Replace_digits(e)).Contains(selectedMod.modContext)) 
                {
                    window.changeRowBackgroundColorListBox(new SolidColorBrush(selectedMod.color), selectedMod.modContext);
                    if (Settings.IsSoundSignalEnabled)
                        SystemSounds.Beep.Play();
                }      
        }

        public void run_program() {


            if (parseClipboard())
            {

                window.Set_Data_Contexts(map);

                Check_mods(_selectedModViewModel.selectedModsToShow, map.mapMods);

                window.Visibility = Visibility.Visible;


                if (window.WindowState == WindowState.Minimized)
                    window.WindowState = WindowState.Normal;

            }
            else window.Visibility = Visibility.Hidden;
        }

        public bool isClipboardLocked() {

            var process = ClipboardHandler.GetProcessLockingClipboard();

            return process.Id != 0;
        }
    }
}
