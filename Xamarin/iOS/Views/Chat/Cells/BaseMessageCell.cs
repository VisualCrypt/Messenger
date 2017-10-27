using System;
using UIKit;
using Foundation;
using CoreGraphics;
using ObsidianMobile.iOS.Controls;

namespace ObsidianMobile.iOS.Views
{
    public abstract class BaseMessageCell : UICollectionViewCell
    {
        protected BaseBubbleView BubbleView { get; private set; }

        public string Text
        {
            get
            {
                return BubbleView.MessageView.MessageLabel.Text;
            }
            set
            {
                BubbleView.MessageView.MessageLabel.Text = value;
            }
        }

        public string Date
        {
            get
            {
                return BubbleView.MessageView.DateOfMessageLabel.Text;
            }
            set
            {
                BubbleView.MessageView.DateOfMessageLabel.Text = value;
            }
        }

        public BaseMessageCell(CGRect frame) : base(frame)
        {
        }

        public BaseMessageCell(IntPtr handle) : base(handle)
        {
            BubbleView = GetBubbleView();
            ContentView.Add(BubbleView);
            ContentView.TranslatesAutoresizingMaskIntoConstraints = false;
        }

        protected abstract BaseBubbleView GetBubbleView();
    }
}