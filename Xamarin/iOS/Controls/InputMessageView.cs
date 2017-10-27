using UIKit;

namespace ObsidianMobile.iOS.Controls
{
    public class InputMessageView : UIView
    {
        int MAX_NUMBER_OF_LINES = 5;

        public UITextView Input { get; set; }
        public UIButton Send { get; set; }

        bool _isNeedUpdateHeightConstraint = true;

        NSLayoutConstraint _inputViewHeight;

        void UpdateInputViewConstraint ()
        {
            _inputViewHeight = NSLayoutConstraint.Create(Input, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1, Input.Frame.Size.Height);
        }

        public InputMessageView()
        {
            Input = new UITextView()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = UIFont.SystemFontOfSize(17),
                ScrollEnabled = false,
                ShowsVerticalScrollIndicator = false,
                ShowsHorizontalScrollIndicator = false,
                ClipsToBounds = true,
            };
            Input.Layer.CornerRadius = 16;
            Input.Layer.BorderColor = UIColor.Black.CGColor;
            Input.Layer.BorderWidth = 1;


            Input.Changed += (sender, e) =>
            {
                var numberOfLines = GetNumberOfLinesInInput();

                if (numberOfLines <= MAX_NUMBER_OF_LINES && !_isNeedUpdateHeightConstraint)
                {
                    ResizeInput();
                }
            };

            Send = new UIButton()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };

            Send.SetImage(UIImage.FromBundle("Send"), UIControlState.Normal);

            AddSubviews(Input, Send);
            InitializeConstraints();
        }

        int GetNumberOfLinesInInput()
        {
            return (int)((Input.ContentSize.Height - Input.TextContainerInset.Top - Input.TextContainerInset.Bottom) / Input.Font.LineHeight);
        }

        private void InitializeConstraints()
        {
            var buttonConstraints = new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(Send,NSLayoutAttribute.Height,NSLayoutRelation.Equal,1,32),
                NSLayoutConstraint.Create(Send,NSLayoutAttribute.Width,NSLayoutRelation.Equal,1,32),
            };
            Send.AddConstraints(buttonConstraints);

            var rootConstraints = new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(this,NSLayoutAttribute.Height,NSLayoutRelation.GreaterThanOrEqual,1,40),//TODO
                NSLayoutConstraint.Create(Input,NSLayoutAttribute.Leading,NSLayoutRelation.Equal,this,NSLayoutAttribute.Leading,1,8),
                NSLayoutConstraint.Create(Input,NSLayoutAttribute.Top,NSLayoutRelation.Equal,this,NSLayoutAttribute.Top,1,4),
                NSLayoutConstraint.Create(this,NSLayoutAttribute.Bottom,NSLayoutRelation.Equal,Input,NSLayoutAttribute.Bottom,1,4),
                NSLayoutConstraint.Create(this,NSLayoutAttribute.Trailing,NSLayoutRelation.Equal,Send,NSLayoutAttribute.Trailing,1,8),
                NSLayoutConstraint.Create(this,NSLayoutAttribute.Bottom,NSLayoutRelation.Equal,Send,NSLayoutAttribute.Bottom,1,4),
                NSLayoutConstraint.Create(Send,NSLayoutAttribute.Leading,NSLayoutRelation.Equal,Input,NSLayoutAttribute.Trailing,1,8),
            };
            AddConstraints(rootConstraints);
        }

        void ResizeInput ()
        {
            var numberOfLines = GetNumberOfLinesInInput();

            if (numberOfLines > MAX_NUMBER_OF_LINES && _isNeedUpdateHeightConstraint)
            {
                UpdateInputViewConstraint();
                Input.ShowsVerticalScrollIndicator = true;
                Input.ScrollEnabled = true;
                Input.AddConstraint(_inputViewHeight);
                _isNeedUpdateHeightConstraint = false;

            }
            else if (numberOfLines <= MAX_NUMBER_OF_LINES && !_isNeedUpdateHeightConstraint)
            {
                Input.ScrollEnabled = false;
                Input.ShowsVerticalScrollIndicator = false;
                Input.RemoveConstraint(_inputViewHeight);
                _isNeedUpdateHeightConstraint = true;
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            ResizeInput();
        }
    }
}
