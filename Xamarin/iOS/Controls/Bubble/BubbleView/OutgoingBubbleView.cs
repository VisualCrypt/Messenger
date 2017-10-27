using UIKit;
using ObsidianMobile.iOS.Controls;

namespace ObsidianMobile.iOS
{
    public class OutgoingBubbleView : BaseBubbleView
    {
        public OutgoingBubbleView()
        {
            MessageView = new OutgoingBubbleMessageView()
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            AddSubviews(MessageView);

            IniitializeConstraints();
        }

        private void IniitializeConstraints()
        {
            var rootConstraints = new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(this,NSLayoutAttribute.Height, NSLayoutRelation.GreaterThanOrEqual,1,48),
                NSLayoutConstraint.Create(MessageView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal,this,NSLayoutAttribute.Leading,1,8),
                NSLayoutConstraint.Create(MessageView, NSLayoutAttribute.Top, NSLayoutRelation.Equal,this,NSLayoutAttribute.Top,1,8),
                NSLayoutConstraint.Create(this, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal,MessageView,NSLayoutAttribute.Bottom,1,8),
                NSLayoutConstraint.Create(this, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal,MessageView,NSLayoutAttribute.Trailing,1,1),
            };
            AddConstraints(rootConstraints);
        }
    }
}
