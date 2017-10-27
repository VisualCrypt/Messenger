using System;
using GalaSoft.MvvmLight.Helpers;

namespace ObsidianMobile.iOS.Views
{
    public class BaseCollectionViewSource<T> : ObservableTableViewSource<T>
    {
        public Action<T> OnRowSelected { get; set; }

        public override void RowSelected(UIKit.UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            base.RowSelected(tableView, indexPath);

            var item = this.SelectedItem;
            OnRowSelected?.Invoke(item);
        }
    }
}
