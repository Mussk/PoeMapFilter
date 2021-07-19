using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Windows;
using PoeMapFilter.ViewModels;
using System.Collections.ObjectModel;
using System.Media;
using Resx = PoeMapFIlter.resources.lang.Resources;

namespace PoeMapFilter
{
    public class MapWindowLogic
    {
        public Map Map { get; set; }

        private MainWindow window;

        public SelectedModViewModel SelectedModViewModel { get; set; }

        public MapWindowLogic(MainWindow window) {

            this.window = window;

            Map = new("Initial_map_name", 0, "Initial_map_atlas_region", 0);

            window.SetDataContexts(Map);
  
            SelectedModViewModel = Serialization<SelectedModViewModel>.
                Deserialize<SelectedModViewModel>("SelectedModViewModel.json");

        }

        public string GetClipboardData()
        {

            string data = "";

            while (IsClipboardLocked()) { }
            
            if (Clipboard.ContainsText())            
                data = Clipboard.GetData(DataFormats.Text).ToString();
            else
                window.Visibility = Visibility.Hidden;

            return data;

        }
        public bool ParseClipboard()
        {
            int offsetCounter = 0;

            var ClipboardRawData = GetClipboardData();

            string[] MapByLines = ClipboardRawData.Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            if (!MapByLines[0].Equals(Resx.ItemClassString)) return false;

            string mapName = MapByLines.ElementAtOrDefault(3);

            int mapTier = Int32.Parse(MapByLines.ElementAtOrDefault(5).Split(' ')[2]);

            string mapAtlasRegion;

            if (this.Map.CheckImplicitMod(MapByLines))
            {
                offsetCounter++;
                mapAtlasRegion = MapByLines.ElementAtOrDefault(13).Split('(')[0].Trim();
            }
            else
            {
                mapAtlasRegion = MapByLines.ElementAtOrDefault(6).Split(':')[1].Trim();
            }
            double mapItemQuantity = ParseMapQuantity(MapByLines.ElementAtOrDefault(7-offsetCounter));

            List<string> mapMods = ParseMapMods(MapByLines);

            this.Map.MapName = mapName;
            this.Map.MapTier = mapTier;
            this.Map.MapAtlasRegion = mapAtlasRegion;
            this.Map.MapItemQuantity = Math.Round(mapItemQuantity * 100, 0);
            this.Map.MapMods = mapMods;

            return true;
        }

        public double ParseMapQuantity(string raw_data) {

            int pFrom = raw_data.IndexOf("+");
            //blue maps???
            int pTo = raw_data.LastIndexOf("% (augmented)");

            return Double.Parse(raw_data.Substring(pFrom, pTo - pFrom)) / 100;

        }

        public List<string> ParseMapMods(string[] MapByLines)
        {
            string current_line = "";

            int index_line = MapByLines.Length - 3;

            List<string> mods = new();

            while (!current_line.Equals(Map.MAP_SEPARATOR)) {

                mods.Add(MapByLines[index_line]);

                index_line--;

                current_line = MapByLines[index_line];

            }

            mods.Reverse();

            return mods;
        }

        public void Check_mods(ObservableCollection<SelectedModViewItem> selectedMods, List<string> map_mods) {

            foreach (var selectedMod in selectedMods)
                if (map_mods.Select(e => PoeWikiJSONIteraction.ReplaceDigits(e)).Contains(selectedMod.ModContext)) 
                {
                    window.ChangeRowBackgroundColorListBox(new SolidColorBrush(selectedMod.Color), selectedMod.ModContext);
                    if (Settings.IsSoundSignalEnabled)
                        SystemSounds.Beep.Play();
                }      
        }

        public void Run_MapWindowLogic() {


            if (ParseClipboard())
            {

                window.SetDataContexts(Map);

                Check_mods(SelectedModViewModel.SelectedModsToShow, Map.MapMods);

                window.Visibility = Visibility.Visible;


                if (window.WindowState == WindowState.Minimized)
                    window.WindowState = WindowState.Normal;

            }
            else window.Visibility = Visibility.Hidden;
        }

        public bool IsClipboardLocked() {

            var process = ClipboardHandler.GetProcessLockingClipboard();

            return process.Id != 0;
        }
    }
}
