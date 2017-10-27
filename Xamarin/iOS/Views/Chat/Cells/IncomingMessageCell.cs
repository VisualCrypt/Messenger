using System;
using ObsidianMobile.iOS.Extensions;
using ObsidianMobile.iOS.Controls;
using CoreGraphics;
using UIKit;

namespace ObsidianMobile.iOS.Views
{
    public class IncomingMessageCell : BaseMessageCell
    {
        public const string Key = "IncomingCell";

        protected new IncomingBubbleView BubbleView => base.BubbleView as IncomingBubbleView;

        public string Name
        {
            get
            {
                return (BubbleView.MessageView as IncomingBubbleMessageView).ProfileNameLabel.Text;
            }
            set
            {
                (BubbleView.MessageView as IncomingBubbleMessageView).ProfileNameLabel.Text = value;
            }
        }

        public IncomingMessageCell(CGRect frame) : base (frame)
        {
        }

        public IncomingMessageCell(IntPtr handle) : base(handle)
        {

            BubbleView.ProfileImageView.Image = UIImage.FromBundle("UserUnselected");

            var screenWidth = UIScreen.MainScreen.Bounds.Size.Width;
            ContentView.AddConstraint(NSLayoutConstraint.Create(ContentView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, 1, screenWidth));

            InitializeConstraints();
        }

        private void InitializeConstraints()
        {
            var contentViewConstraints = new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(BubbleView,NSLayoutAttribute.Top, NSLayoutRelation.Equal,ContentView,NSLayoutAttribute.Top,1,0),
                NSLayoutConstraint.Create(ContentView,NSLayoutAttribute.Trailing, NSLayoutRelation.GreaterThanOrEqual,BubbleView,NSLayoutAttribute.Trailing,1,0),
                NSLayoutConstraint.Create(BubbleView,NSLayoutAttribute.Leading, NSLayoutRelation.Equal,ContentView,NSLayoutAttribute.Leading,1,0),
                NSLayoutConstraint.Create(ContentView,NSLayoutAttribute.Bottom, NSLayoutRelation.Equal,BubbleView,NSLayoutAttribute.Bottom,1,0),
            };

            ContentView.AddConstraints(contentViewConstraints);

            var constraints = new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(ContentView,NSLayoutAttribute.Top, NSLayoutRelation.Equal,this,NSLayoutAttribute.Top,1,0),
                NSLayoutConstraint.Create(this,NSLayoutAttribute.Trailing, NSLayoutRelation.Equal,ContentView,NSLayoutAttribute.Trailing,1,0),
                NSLayoutConstraint.Create(ContentView,NSLayoutAttribute.Leading, NSLayoutRelation.Equal,this,NSLayoutAttribute.Leading,1,0),
                NSLayoutConstraint.Create(this,NSLayoutAttribute.Bottom, NSLayoutRelation.Equal,ContentView,NSLayoutAttribute.Bottom,1,0),
            };
            AddConstraints(constraints);
        }

        public override void LayoutIfNeeded()
        {
            base.LayoutIfNeeded();

            BubbleView.ProfileImageView.MakeRoundImageView();
        }

        protected override BaseBubbleView GetBubbleView()
        {
            return new IncomingBubbleView();
        }
    }
}