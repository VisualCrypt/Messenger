using UIKit;

namespace ObsidianMobile.iOS.Controls
{
    public class BaseBubbleMessageView : UIView
    {
        public BubbleDecoratorView DecorView { get; set; }
        public ActiveLabel MessageLabel { get; set; }
        public UILabel DateOfMessageLabel { get; set; }

        public BaseBubbleMessageView()
        {
            TranslatesAutoresizingMaskIntoConstraints = false;

            DecorView = new BubbleDecoratorView()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };

            MessageLabel = new ActiveLabel()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.Clear,
                Lines = 0,
                Font = UIFont.SystemFontOfSize(17),
            };

            DateOfMessageLabel = new UILabel()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.Clear,
                Lines = 0,
                Font = UIFont.SystemFontOfSize(14),
                TextAlignment = UITextAlignment.Right,
            };

            AddSubviews(DecorView, DateOfMessageLabel, MessageLabel);
        }
    }
}
