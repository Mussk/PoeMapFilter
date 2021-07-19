using System.Windows.Media;

namespace PoeMapFilter.ViewModels
{
    public class SelectedModViewItem
    {
        public string ModContext { get; set; }

        public Color Color { get; set; }

        public static readonly Color DefaultColor = Colors.Red;

        public SelectedModViewItem(string ModContext, Color Color)
        {
            this.ModContext = ModContext;
            this.Color = Color;
        }
    }
}
