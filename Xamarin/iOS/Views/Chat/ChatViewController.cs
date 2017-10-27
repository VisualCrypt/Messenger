using System;
using CoreGraphics;
using Foundation;
using GalaSoft.MvvmLight.Helpers;
using GalaSoft.MvvmLight.Ioc;
using ObsidianMobile.Core.Enums;
using ObsidianMobile.Core.Extensions;
using ObsidianMobile.Core.Interfaces.Models;
using ObsidianMobile.Core.Interfaces.ViewModels;
using ObsidianMobile.iOS.Controls;
using UIKit;

namespace ObsidianMobile.iOS.Views
{
    public class ChatViewController : BaseViewController
    {
        IChatViewModel ViewModel
        {
            get
            {
                return (IChatViewModel)_viewModel;
            }
        }

        public UICollectionView MessagesCollection { get; set; }
        public InputMessageView InputMessage { get; set; }

        NSLayoutConstraint bottomConstraint;

        NSObject willShowToken;
        NSObject willHideToken;

        UIButton SendButton
        {
            get
            {
                return InputMessage.Send;
            }
        }

        UITextView TextView
        {
            get
            {
                return InputMessage.Input;
            }
        }

        public ChatViewController(int chatId)
        {
            _viewModel = SimpleIoc.Default.GetInstance<IChatViewModel>();

            ViewModel.ChatId = chatId;

            _viewModel.OnStart();

            var edit = new UIBarButtonItem(UIBarButtonSystemItem.Edit, (sender, e) =>
            {
                ViewModel.NavigateToContactDetailsCommand.Execute(ViewModel.RecipientContact);
            });

            edit.TintColor = UIColor.FromRGB(34, 209, 194);

            NavigationItem.RightBarButtonItem = edit;

            Initialize();
        }

        void Initialize()
        {
            var source = ViewModel.Messages.GetCollectionViewSource<IMessage, BaseMessageCell>(BindMessageCell, null, null, () => new ChatCollectionViewSource());

            source.SelectionChanged += (sender, e) =>
            {
                var item = source.SelectedItem;
            };

            MessagesCollection = new UICollectionView(new CGRect(0, 0, 0, 0), new ChatLayout())
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };

            MessagesCollection.BackgroundColor = UIColor.FromRGB(190,234,232);
            MessagesCollection.RegisterClassForCell(typeof(IncomingMessageCell), IncomingMessageCell.Key);
            MessagesCollection.RegisterClassForCell(typeof(OutgoingMessageCell), OutgoingMessageCell.Key);

            MessagesCollection.Source = source;

            InputMessage = new InputMessageView()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.FromRGB(232, 232, 232)
            };

            TextView.Started += OnTextViewStarted;
            TextView.Changed += OnTextChanged;

            View.AddSubviews(MessagesCollection, InputMessage);

            MessagesCollection.AddGestureRecognizer(new UITapGestureRecognizer(DismissKeyboard));

            InitializeConstraints();
        }

        void BindMessageCell(BaseMessageCell cell, IMessage item, NSIndexPath indexPath)
        {
            if (item.Type == MessageType.Incoming)
            {
                //TODO only for direct chat
                (cell as IncomingMessageCell).Name = ViewModel.RecipientContact.Name;
            }

            cell.Text = item.Text;
            cell.Date = item.Date.ToMessageStyleString();
        }

        protected override void SetBindings()
        {
            base.SetBindings();

            _bindings.Add(this.SetBinding(() => ViewModel.CurrentMessage, () => TextView.Text, BindingMode.TwoWay));
            SendButton.SetCommand(ViewModel.SendTextMessageCommand);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            UpdateSendButtonState();
            willShowToken = UIKeyboard.Notifications.ObserveWillShow(KeyboardWillShowHandler);
            willHideToken = UIKeyboard.Notifications.ObserveWillHide(KeyboardWillHideHandler);

            NavigationItem.Title = ViewModel.ChatName;
        }

        private void InitializeConstraints()
        {
            bottomConstraint = NSLayoutConstraint.Create(View, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, InputMessage, NSLayoutAttribute.Bottom, 1, 0);
            var constraints = new NSLayoutConstraint[]
            {
                bottomConstraint,

                NSLayoutConstraint.Create(MessagesCollection,NSLayoutAttribute.Leading, NSLayoutRelation.Equal,View,NSLayoutAttribute.Leading,1,0),
                NSLayoutConstraint.Create(View,NSLayoutAttribute.Trailing, NSLayoutRelation.Equal,InputMessage,NSLayoutAttribute.Trailing,1,0),
                NSLayoutConstraint.Create(View,NSLayoutAttribute.Trailing, NSLayoutRelation.Equal,MessagesCollection,NSLayoutAttribute.Trailing,1,0),
                NSLayoutConstraint.Create(View,NSLayoutAttribute.Top, NSLayoutRelation.Equal,MessagesCollection,NSLayoutAttribute.Top,1,0),
                NSLayoutConstraint.Create(InputMessage,NSLayoutAttribute.Leading, NSLayoutRelation.Equal,View,NSLayoutAttribute.Leading,1,0),
                NSLayoutConstraint.Create(InputMessage,NSLayoutAttribute.Top, NSLayoutRelation.Equal,MessagesCollection,NSLayoutAttribute.Bottom,1,0),
            };
            View.AddConstraints(constraints);
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            //TODO
            //ScrollToBottom(false);
        }

        void ScrollToBottom(bool animated)
        {
            if (MessagesCollection.NumberOfSections() == 0)
                return;

            var items = (int)MessagesCollection.NumberOfItemsInSection(0);
            if (items == 0)
                return;

            var finalRow = (int)NMath.Max(0, MessagesCollection.NumberOfItemsInSection(0) - 1);
            NSIndexPath finalIndexPath = NSIndexPath.FromRowSection(finalRow, 0);
            MessagesCollection.ScrollToItem(finalIndexPath, UICollectionViewScrollPosition.Top, animated);
        }

        void KeyboardWillShowHandler(object sender, UIKeyboardEventArgs e)
        {
            UpdateButtomLayoutConstraint(e);
        }

        void KeyboardWillHideHandler(object sender, UIKeyboardEventArgs e)
        {
            UpdateButtomLayoutConstraint(e);
        }

        void OnTextViewStarted(object sender, EventArgs e)
        {
            ScrollToBottom(true);
        }

        void OnTextChanged(object sender, EventArgs e)
        {
            UpdateSendButtonState();
        }

        void UpdateSendButtonState()
        {
            SendButton.Enabled = !string.IsNullOrWhiteSpace(TextView.Text);
        }

        void UpdateButtomLayoutConstraint(UIKeyboardEventArgs e)
        {
            UIViewAnimationCurve curve = e.AnimationCurve;
            UIView.Animate(e.AnimationDuration, 0, ConvertToAnimationOptions(e.AnimationCurve), () =>
            {
                var keyboardSize = UIKeyboard.FrameEndFromNotification(e.Notification);
                SetInputViewBottomConstraint(keyboardSize.Height);
            }, null);
        }

        UIViewAnimationOptions ConvertToAnimationOptions(UIViewAnimationCurve curve)
        {
            //http://stackoverflow.com/questions/18870447/how-to-use-the-default-ios7-uianimation-curve/18873820#18873820
            return (UIViewAnimationOptions)((int)curve << 16);
        }

        void SetInputViewBottomConstraint(nfloat constant)
        {
            bottomConstraint.Constant = constant;
            View.SetNeedsUpdateConstraints();
            View.LayoutIfNeeded();
        }

        void DismissKeyboard()
        {
            TextView.ResignFirstResponder();
            SetInputViewBottomConstraint(0);
            View.SetNeedsUpdateConstraints();
            View.LayoutIfNeeded();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            willShowToken.Dispose();
            willHideToken.Dispose();
        }
    }
}
