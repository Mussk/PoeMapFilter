using System.Collections.Generic;

namespace PoeMapFilter
{
    public class Map
    {
        public string MapName { get; set; }
        public int MapTier { get; set; }
        public string MapAtlasRegion { get; set; }
        public double MapItemQuantity { get; set; }
        public List<string> MapMods { get; set; }

        public static readonly string MAP_SEPARATOR = "--------";

        //for first initialization
        public Map(string MapName, int MapTier, string MapAtlasRegion, double MapItemQuantity)
        {
            this.MapName = MapName;
            this.MapTier = MapTier;
            this.MapAtlasRegion = MapAtlasRegion;
            this.MapItemQuantity = MapItemQuantity;
            this.MapMods = createInitialModsList();
        }

        public Map() {}

        private List<string> createInitialModsList() {

            var res = new List<string>();
            res.Add("sampleData");

            return res;
        }

        public bool CheckImplicitMod(string[] MapByLines) {

            int count = 0;

            foreach (var line in MapByLines)
                if (line.Equals(MAP_SEPARATOR))
                    count++;

            return count == 5;
        }
    }
}
