using Foundation;
using GalaSoft.MvvmLight.Helpers;
using ObsidianMobile.Core.Enums;
using ObsidianMobile.Core.Interfaces.Models;
using UIKit;

namespace ObsidianMobile.iOS.Views
{
    public class ChatCollectionViewSource : ObservableCollectionViewSource<IMessage, BaseMessageCell>
    {
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var item = this.GetItem(indexPath) as IMessage;

            var reuseId = item.Type == MessageType.Outgoing ? OutgoingMessageCell.Key : IncomingMessageCell.Key;

            var messageCell = collectionView.DequeueReusableCell(reuseId, indexPath);

            BindCellDelegate?.Invoke(messageCell as BaseMessageCell, item, indexPath);

            return messageCell as BaseMessageCell;
        }
    }
}