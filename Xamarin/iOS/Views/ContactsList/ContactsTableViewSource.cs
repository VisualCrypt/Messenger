using Foundation;
using ObsidianMobile.Core.Interfaces.Models;
using UIKit;

namespace ObsidianMobile.iOS.Views
{
    public class ContactsTableViewSource : BaseCollectionViewSource<IContact>
    {
        public override UITableViewCell GetCell(UITableView view, NSIndexPath indexPath)
        {
            var cell = (ContactCell)view.DequeueReusableCell(ContactCell.Key, indexPath);

            var item = this.GetItem(indexPath) as IContact;

            cell.BackgroundColor = UIColor.FromRGB(190, 234, 232);

            cell.ProfileStatus = "Online";
            cell.Name = item.Name;

            return cell;
        }
    }
}