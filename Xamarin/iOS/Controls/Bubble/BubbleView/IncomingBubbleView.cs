using ObsidianMobile.iOS.Controls;
using UIKit;

namespace ObsidianMobile.iOS
{
    public class IncomingBubbleView : BaseBubbleView
    {
        public ProfileImageView ProfileImageView { get; set; }

        public IncomingBubbleView()
        {
            ProfileImageView = new ProfileImageView()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };

            MessageView = new IncomingBubbleMessageView()
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            AddSubviews(ProfileImageView,MessageView);

            IniitializeConstraints();
        }

        private void IniitializeConstraints()
        {
            var rootConstraints = new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(this,NSLayoutAttribute.Height, NSLayoutRelation.GreaterThanOrEqual,1,48),
                NSLayoutConstraint.Create(ProfileImageView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal,this,NSLayoutAttribute.Leading,1,1),
                NSLayoutConstraint.Create(this, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal,ProfileImageView,NSLayoutAttribute.Bottom,1,8),
                NSLayoutConstraint.Create(MessageView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal,ProfileImageView,NSLayoutAttribute.Trailing,1,4),
                NSLayoutConstraint.Create(MessageView, NSLayoutAttribute.Top, NSLayoutRelation.Equal,this,NSLayoutAttribute.Top,1,8),
                NSLayoutConstraint.Create(this, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal,MessageView,NSLayoutAttribute.Bottom,1,8),
                NSLayoutConstraint.Create(this, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal,MessageView,NSLayoutAttribute.Trailing,1,8),
            };
            AddConstraints(rootConstraints);
        }
    }
}
