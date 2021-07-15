using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace PoeBadMapsMod.ViewModels
{

    public class SelectedModViewModel : IDropTarget
    {

        public ObservableCollection<SelectedModViewItem> selectedModsToShow { get; set; }

        public SelectedModViewModel()
        { 
           selectedModsToShow = new ObservableCollection<SelectedModViewItem>();       
        }

        public void DragOver(IDropInfo dropInfo)
        {
            string sourceItem = dropInfo.Data as string;
            
            if (sourceItem != null)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            selectedModsToShow.Add(new SelectedModViewItem((string)dropInfo.Data, SelectedModViewItem.DefaultColor));
           
        }
    }
}
