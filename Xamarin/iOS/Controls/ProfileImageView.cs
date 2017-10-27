using UIKit;

namespace ObsidianMobile.iOS.Controls
{
    public class ProfileImageView : UIImageView
    {
        public ProfileImageView()
        {
            InitializeConstrints();
        }

        private void InitializeConstrints()
        {
            var constraints = new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(this, NSLayoutAttribute.Height, NSLayoutRelation.Equal,1,40),
                NSLayoutConstraint.Create(this, NSLayoutAttribute.Width, NSLayoutRelation.Equal, this, NSLayoutAttribute.Height, 1,0),
            };
            AddConstraints(constraints);
        }
    }
}