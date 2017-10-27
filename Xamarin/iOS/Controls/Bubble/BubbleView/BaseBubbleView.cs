using UIKit;

namespace ObsidianMobile.iOS.Controls
{
    public class BaseBubbleView : UIView
    {
        public BaseBubbleMessageView MessageView { get; set; }

        public BaseBubbleView()
        {
            TranslatesAutoresizingMaskIntoConstraints = false;
        }
    }
}