using GongSolutions.Wpf.DragDrop;
using System.Collections.ObjectModel;
using System.Windows;

namespace PoeMapFilter.ViewModels
{

    public class SelectedModViewModel : IDropTarget
    {

        public ObservableCollection<SelectedModViewItem> SelectedModsToShow { get; set; }

        public SelectedModViewModel()
        { 
           SelectedModsToShow = new ObservableCollection<SelectedModViewItem>();       
        }

        public void DragOver(IDropInfo dropInfo)
        {   
            if (dropInfo.Data is not null)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            SelectedModsToShow.Add(
                new SelectedModViewItem(
                    (string)dropInfo.Data,
                    SelectedModViewItem.DefaultColor)
                );
        }
    }
}
