using System;
using UIKit;
using CoreGraphics;
using ObsidianMobile.iOS.Controls;

namespace ObsidianMobile.iOS.Views
{
    public class OutgoingMessageCell : BaseMessageCell
    {
        public const string Key = "OutgoingCell";

        protected new OutgoingBubbleView BubbleView => base.BubbleView as OutgoingBubbleView;

        public OutgoingMessageCell(CGRect frame) : base(frame)
        {
        }

        public OutgoingMessageCell(IntPtr handle) : base(handle)
        {
            var screenWidth = UIScreen.MainScreen.Bounds.Size.Width;
            ContentView.AddConstraint(NSLayoutConstraint.Create(ContentView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, 1, screenWidth));

            InitializeConstraints();
        }

        private void InitializeConstraints()
        {
            var contentViewConstraints = new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(BubbleView,NSLayoutAttribute.Top, NSLayoutRelation.Equal,ContentView,NSLayoutAttribute.Top,1,0),
                NSLayoutConstraint.Create(ContentView,NSLayoutAttribute.Trailing, NSLayoutRelation.Equal,BubbleView,NSLayoutAttribute.Trailing,1,0),
                NSLayoutConstraint.Create(BubbleView,NSLayoutAttribute.Leading, NSLayoutRelation.GreaterThanOrEqual,ContentView,NSLayoutAttribute.Leading,1,0),
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

        protected override BaseBubbleView GetBubbleView()
        {
            return new OutgoingBubbleView();
        }
    }
}