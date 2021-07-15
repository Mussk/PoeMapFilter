using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeBadMapsMod.ViewModels
{
    public class SelectedModViewItem
    {
        public string modContext { get; set; }

        public Color color { get; set; }

        public static readonly Color DefaultColor = Colors.Red;

        public SelectedModViewItem(string modContext, Color color)
        {
            this.modContext = modContext;
            this.color = color;
        }
    }
}
