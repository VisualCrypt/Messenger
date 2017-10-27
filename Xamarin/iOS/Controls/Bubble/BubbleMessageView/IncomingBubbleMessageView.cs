using UIKit;

namespace ObsidianMobile.iOS.Controls
{
    public class IncomingBubbleMessageView : BaseBubbleMessageView
    {
        public UILabel ProfileNameLabel { get; set; }

        public IncomingBubbleMessageView()
        {
            ProfileNameLabel = new UILabel()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.Clear,
                Lines = 0,
                Font = UIFont.SystemFontOfSize(17),
                TextColor = UIColor.FromRGB(57,116,111),
            };
            AddSubview(ProfileNameLabel);

            DecorView.CreateBubbleDecorFromImage(UIColor.White, UIImage.FromBundle("bubble_incoming"));

            InitializeConstraints();
        }

        private void InitializeConstraints()
        {

            var constrints = new NSLayoutConstraint[]{
                NSLayoutConstraint.Create(this,NSLayoutAttribute.Height,NSLayoutRelation.GreaterThanOrEqual,1,44),

                NSLayoutConstraint.Create(DateOfMessageLabel, NSLayoutAttribute.Top, NSLayoutRelation.Equal,MessageLabel,NSLayoutAttribute.Bottom,1,4),

                NSLayoutConstraint.Create(MessageLabel, NSLayoutAttribute.Top, NSLayoutRelation.Equal,ProfileNameLabel,NSLayoutAttribute.Bottom,1,8),

                NSLayoutConstraint.Create(this, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal,MessageLabel,NSLayoutAttribute.Trailing,1,8),

                NSLayoutConstraint.Create(MessageLabel, NSLayoutAttribute.Leading, NSLayoutRelation.Equal,this,NSLayoutAttribute.Leading,1,16),

                NSLayoutConstraint.Create(DecorView,NSLayoutAttribute.Leading,NSLayoutRelation.Equal,this,NSLayoutAttribute.Leading,1,0),
                NSLayoutConstraint.Create(DecorView,NSLayoutAttribute.Top,NSLayoutRelation.Equal,this,NSLayoutAttribute.Top,1,0),
                NSLayoutConstraint.Create(this,NSLayoutAttribute.Trailing,NSLayoutRelation.Equal,DecorView,NSLayoutAttribute.Trailing,1,0),
                NSLayoutConstraint.Create(this,NSLayoutAttribute.Bottom,NSLayoutRelation.Equal,DecorView,NSLayoutAttribute.Bottom,1,0),

                NSLayoutConstraint.Create(this,NSLayoutAttribute.Trailing,NSLayoutRelation.Equal,ProfileNameLabel,NSLayoutAttribute.Trailing,1,8),
                NSLayoutConstraint.Create(ProfileNameLabel,NSLayoutAttribute.Top,NSLayoutRelation.Equal,this,NSLayoutAttribute.Top,1,8),
                NSLayoutConstraint.Create(ProfileNameLabel,NSLayoutAttribute.Leading,NSLayoutRelation.Equal,this,NSLayoutAttribute.Leading,1,16),

                NSLayoutConstraint.Create(this,NSLayoutAttribute.Trailing,NSLayoutRelation.Equal,DateOfMessageLabel,NSLayoutAttribute.Trailing,1,8),
                NSLayoutConstraint.Create(this,NSLayoutAttribute.Bottom,NSLayoutRelation.Equal,DateOfMessageLabel,NSLayoutAttribute.Bottom,1,8),
                NSLayoutConstraint.Create(DateOfMessageLabel,NSLayoutAttribute.Leading,NSLayoutRelation.Equal,this,NSLayoutAttribute.Leading,1,16),
            };
            AddConstraints(constrints);
        }
    }
}
