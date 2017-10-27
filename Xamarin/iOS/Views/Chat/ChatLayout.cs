using CoreGraphics;
using UIKit;

namespace ObsidianMobile.iOS.Views
{
    public class ChatLayout : UICollectionViewFlowLayout
    {
        public ChatLayout()
        {
            ScrollDirection = UICollectionViewScrollDirection.Vertical;
            EstimatedItemSize = new CGSize(1, 1);
            MinimumLineSpacing = 0;
        }
    }
}