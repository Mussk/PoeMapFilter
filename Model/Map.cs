using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeBadMapsMod
{
    public class Map
    {
        public string mapName { get; set; }
        public int mapTier { get; set; }
        public string mapAtlasRegion { get; set; }
        public double mapItemQuantity { get; set; }
        public List<string> mapMods { get; set; }

        public static string MAP_SEPARATOR = "--------";

        //for first initialization
        public Map(string mapName, int mapTier, string mapAtlasRegion, double mapItemQuantity)
        {
            this.mapName = mapName;
            this.mapTier = mapTier;
            this.mapAtlasRegion = mapAtlasRegion;
            this.mapItemQuantity = mapItemQuantity;
            this.mapMods = createInitialModsList();
        }

        public Map() { }

        private List<string> createInitialModsList() {

            var res = new List<string>();
            res.Add("sampleData");

            return res;
        }

        public bool checkImplicitMod(string[] map_by_lines) {

            int count = 0;

            foreach (var line in map_by_lines)
                if (line.Equals("--------"))
                    count++;

            return count == 5;
        }
    }
}
