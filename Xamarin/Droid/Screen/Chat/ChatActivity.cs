using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Widget;
using Com.Lilarcor.Cheeseknife;
using GalaSoft.MvvmLight.Helpers;
using ObsidianMobile.Core;
using ObsidianMobile.Core.Interfaces.ViewModels;
using ObsidianMobile.Core.Interfaces.Views;
using ObsidianMobile.Droid.Chat.Adapter;
using ObsidianMobile.Droid.Chat.Test.Model;
using ObsidianMobile.Droid.Core;
using ObsidianMobile.Droid.Core.Extensions;
using ObsidianMobile.Droid.Screen;

namespace ObsidianMobile.Droid.Chat
{
    [Activity(Label = "Chat", WindowSoftInputMode = Android.Views.SoftInput.AdjustNothing)]
    public partial class ChatActivity : BaseActivity<IChatViewModel>, IChatView
    {
        KeyboardHeightObserver KeyboardObserver;

        //TODO add separate classes for states
        bool _isEmojiKeyboardExplicitlyRevealed;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_chat);
            Cheeseknife.Inject(this);
            SetSupportActionBar(Toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            ViewModel.View = this;
            if (Intent.Extras != null)
            {
                ViewModel.ChatId = Intent.Extras.GetInt(DroidNavigationService.ParameterKey);
            }

            KeyboardObserver = new KeyboardHeightObserver(this);
            KeyboardObserver.KeyboardHeightMeasured += OnKeyboardHeightMeasured;
            MessageInput.Post(KeyboardObserver.Start);

            InitClicks();
            InitBindings();
        }

        public override void OnBackPressed()
        {
            if (_isEmojiKeyboardExplicitlyRevealed)
            {
                EmojiKeyboard.MakeGone();
                _isEmojiKeyboardExplicitlyRevealed = false;
            }
            else
            {
                base.OnBackPressed();
            }
        }

        void InitBindings()
        {
            Bindings.Add(this.SetBinding(() => MessageInput.Text, () => ViewModel.CurrentMessage, BindingMode.TwoWay));

            //TODO chat name
            //Bindings.Add(this.SetBinding(() => ViewModel.ChatName, () => ChatName.Text, BindingMode.OneWay));

            SendButton.SetCommand(nameof(Button.Click), ViewModel.SendTextMessageCommand);
        }

        void InitClicks()
        {
            MessageInput.Touch += (s, e) =>
            {
                if (e.Event.Action == Android.Views.MotionEventActions.Up)
                {
                    EmojiKeyboard.MakeInvisible();
                    MessageInput.RequestFocus();
                    MessageInput.ShowKeyboard();
                }
            };

            ShowEmoji.Click += (s, e) =>
            {
                _isEmojiKeyboardExplicitlyRevealed = !_isEmojiKeyboardExplicitlyRevealed;

                if (_isEmojiKeyboardExplicitlyRevealed)
                {
                    MessageInput.HideKeyboard();
                    EmojiKeyboard.MakeVisible();
                }
                else
                {
                    MessageInput.ShowKeyboard();
                    EmojiKeyboard.MakeInvisible();
                    _isEmojiKeyboardExplicitlyRevealed = false;
                }
            };

            EmojiKeyboard.EmojiClick += (s, e) =>
            {
                MessageInput.Text += e.EmojiText;
                MessageInput.SetSelection(MessageInput.Text.Length);
            };

            ChatImage.SetCommand(nameof(ImageView.Click), ViewModel.NaviagteToChatCommand);
        }

        void OnKeyboardHeightMeasured(object s, KeyboardHeightEventArgs e)
        {
            if (_isEmojiKeyboardExplicitlyRevealed)
            {
                return;
            }

            KeyboardObserver.KeyboardHeightMeasured -= OnKeyboardHeightMeasured;
            EmojiKeyboard.SetHeight(e.Height);
            KeyboardObserver.KeyboardHeightMeasured += OnKeyboardHeightMeasured;
        }

        void OnLinkClick(object s, MessageLinkClickEventArgs e)
        {
            //TODO may be it's better to extract UrlHelperService and ToastService 
            //and call em from shared logic
            if (!UrlHelper.OpenUrl(this, e.Link))
            {
                Toast.MakeText(this, "Unable to open " + e.Link, ToastLength.Short).Show();
            }
        }

        public void OnChatLoaded()
        {
            ViewModel.Messages.CollectionChanged += (s, e) => MessagesRecycler.SmoothScrollToPosition(ViewModel.Messages.Count);

            var adapterDelegate = new ChatAdapterDelegate(new ChatItemViewTypeResolver(ViewModel.Messages), Server.CURRENT_USER_ID);
            adapterDelegate.LinkClick += OnLinkClick;
            var adapter = ViewModel.Messages.GetRecyclerAdapter(adapterDelegate.OnBindViewHolder, adapterDelegate.OnCreateViewHolder);
            //HasStableIds to avoud juddering on updates
            adapter.HasStableIds = true;

            MessagesRecycler.SetAdapter(adapter);
            MessagesRecycler.SetLayoutManager(new LinearLayoutManager(this) { StackFromEnd = true });
            MessagesRecycler.SetItemAnimator(new DefaultItemAnimator());
        }
    }
}
