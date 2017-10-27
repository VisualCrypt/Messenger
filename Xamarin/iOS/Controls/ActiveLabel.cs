using System;
using System.Collections.Generic;
using Foundation;
using ObsidianMobile.Core.Utils;
using UIKit;

namespace ObsidianMobile.iOS.Controls
{
    public class ActiveLabel : UILabel
    {
        private UIColor hashtagColor = UIColor.Red;
        private UIColor urlColor = UIColor.Blue;
        private UIColor userNameColor = UIColor.Purple;

        private NSLayoutManager layoutManager;
        private NSTextContainer textContainer;
        private NSTextStorage textStorage;

        private string textMessage;

        private List<ActiveItem> activeItems;
        private ActiveItemRegex regex;

        public override string Text
        {
            get
            {
                return textMessage;
            }
            set
            {
                textMessage = value;
                activeItems = regex.GetAllElements(value);
                base.AttributedText = GetAttributedString(value);
            }
        }

        public override UILineBreakMode LineBreakMode
        {
            get
            {
                return base.LineBreakMode;
            }
            set
            {
                textContainer.LineBreakMode = value;
                base.LineBreakMode = value;
            }
        }

        public override nint Lines
        {
            get
            {
                return base.Lines;
            }
            set
            {
                textContainer.MaximumNumberOfLines = (nuint)value;
                base.Lines = value;
            }
        }

        public ActiveLabel()
        {
            regex = new ActiveItemRegex();

            UserInteractionEnabled = true;

            var tapGestureRecognizer = new UITapGestureRecognizer();

            tapGestureRecognizer.AddTarget((obj) => SelectActiveLabel(obj as UITapGestureRecognizer));

            AddGestureRecognizer(tapGestureRecognizer);

            SetupLabel();
        }

        private void SelectActiveLabel(UITapGestureRecognizer gestureRecognizer)
        {
            textContainer.Size = Bounds.Size;
            textContainer.MaximumNumberOfLines = (nuint)Lines;

            ((ActiveLabel)gestureRecognizer.View).Lines = Lines;
            ((ActiveLabel)gestureRecognizer.View).Font = Font;

            textStorage.Append(AttributedText);

            var location = gestureRecognizer.LocationInView(this);

            nfloat s = 0f;
            var characterIndex = (int)layoutManager.CharacterIndexForPoint(location, textContainer, ref s);
            foreach(var item in activeItems)
            {
                if ( characterIndex >= item.StartIndex && characterIndex <= item.LastIndex)
                {
                    item.Select();
                    TestSelect(item);
                    break;
                }
            }
        }

        private void TestSelect (ActiveItem item)
        {
            UIAlertView alert = new UIAlertView();
            alert.Title = item.Type.ToString();
            alert.AddButton("OK");
            alert.Message = item.Text;
            alert.Show();
        }

        private void SetupLabel ()
        {
            textStorage = new NSTextStorage();
            layoutManager = new NSLayoutManager();
            textContainer = new NSTextContainer();

            textStorage.AddLayoutManager(layoutManager);
            layoutManager.AddTextContainer(textContainer);
            textContainer.LineFragmentPadding = 0;
            textContainer.LineBreakMode = LineBreakMode;
        }

        private NSMutableAttributedString GetAttributedString(string text)
        {
            var attributedString = new NSMutableAttributedString(text);

            foreach (var item in activeItems)
            {
                var range = new NSRange(item.StartIndex, item.Text.Length);
                var colorItem = UIColor.Black;
                var attributes = new UIStringAttributes();
                switch (item.Type)
                {
                    case RegexType.Username:
                        colorItem = userNameColor;
                        break;
                    case RegexType.Hashtag:
                        colorItem = hashtagColor;
                        break;
                    case RegexType.Url:
                        colorItem = urlColor;
                        attributes.Link = new NSUrl(item.Text);
                        break;
                }
                attributes.ForegroundColor = colorItem;
                attributedString.SetAttributes(attributes.Dictionary, range);
            }

            return attributedString;
        }
    }
}
