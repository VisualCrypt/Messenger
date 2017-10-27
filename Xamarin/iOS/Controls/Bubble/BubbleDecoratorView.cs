using CoreGraphics;
using UIKit;

namespace ObsidianMobile.iOS.Controls
{
    public class BubbleDecoratorView : UIView
    {
        public UIImageView DecorImageView { get; set; }

        public BubbleDecoratorView()
        {

            DecorImageView = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            Add(DecorImageView);
            InitializeConstraints();
        }

        public void CreateBubbleDecorFromImage(UIColor color, UIImage mask)
        {
            var cap = new UIEdgeInsets
            {
                Top = 20f,
                Left = 20f,
                Bottom = 20f,
                Right = 20f
            };

            DecorImageView.Image = CreateColoredImage(color,mask).CreateResizableImage(cap);
        }

        private void InitializeConstraints()
        {
            DecorImageView.AddConstraint(NSLayoutConstraint.Create(DecorImageView, NSLayoutAttribute.Height, NSLayoutRelation.GreaterThanOrEqual, 1, 44));

            var constraints = new NSLayoutConstraint[]{
                NSLayoutConstraint.Create(this, NSLayoutAttribute.Height, NSLayoutRelation.GreaterThanOrEqual, 1, 44),
                NSLayoutConstraint.Create(DecorImageView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, this, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(DecorImageView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(this, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, DecorImageView, NSLayoutAttribute.Trailing, 1, 0),
                NSLayoutConstraint.Create(this, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, DecorImageView, NSLayoutAttribute.Bottom, 1, 0),
            };
            AddConstraints(constraints);
        }

        private UIImage CreateColoredImage(UIColor color, UIImage mask)
        {
            var rect = new CGRect(CGPoint.Empty, mask.Size);
            UIGraphics.BeginImageContextWithOptions(mask.Size, false, mask.CurrentScale);
            CGContext context = UIGraphics.GetCurrentContext();
            mask.DrawAsPatternInRect(rect);
            context.SetFillColor(color.CGColor);
            context.SetBlendMode(CGBlendMode.SourceAtop);
            context.FillRect(rect);
            UIImage result = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return result;
        }
    }
}
