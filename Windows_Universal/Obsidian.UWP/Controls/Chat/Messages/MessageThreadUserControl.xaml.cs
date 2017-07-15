using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Obsidian.Applications.Models.Chat;
using Obsidian.Applications.Services.Interfaces;
using Obsidian.Applications.ViewModels.Chat;
using Obsidian.Common;
using Obsidian.Cryptography.Api.Implementations;
using Obsidian.UWP.Controls.Chat.Messages.Bubbles;

namespace Obsidian.UWP.Controls.Chat.Messages
{
    public sealed partial class MessageThreadUserControl : IMessagesView
    {
        readonly MessagesViewModel _messagesViewModel;
        readonly IFileService _fileService;
        readonly IPhotoImportService _photoImportService;

        bool _isBusy;
        BubbleBase _previousBubbleThatWasAddedAtBottom;

        public MessageThreadUserControl()
        {
            InitializeComponent();
            _messagesViewModel = Application.Current.GetContainer().Get<MessagesViewModel>();
            _messagesViewModel.View = this;
            _fileService = Application.Current.GetContainer().Get<IFileService>();
            _photoImportService = Application.Current.GetContainer().Get<IPhotoImportService>();
            ScrollViewer.ViewChanged += ScrollViewer_ViewChanged;

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }



        public async Task OnThreadLoaded(IReadOnlyCollection<Message> messages)
        {
            MessageThread.Items?.Clear();
            ScrollViewer.ChangeView(null, 0, null, true);
            _previousBubbleThatWasAddedAtBottom = null;

            foreach (var message in messages)
            {
                var bubble = await CreateBubbleFromMessage(message);
                bool scrollToBottom = message == messages.Last();
                AddBubbleToThreadAndScrollToBottom(bubble, scrollToBottom);
            }
        }

        public async Task OnMessageAddedAsync(Message message)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var bubble = await CreateBubbleFromMessage(message);
                AddBubbleToThreadAndScrollToBottom(bubble, true);
            });

        }

        public async Task OnMessageDecrypted(Message message)
        {
            if (MessageThread.Items == null)
                return;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var lastItem = (BubbleBase)MessageThread.Items?.Last();
                foreach (var item in MessageThread.Items)
                {
                    var bubble = (BubbleBase)item;
                    if (bubble.MessageID != message.Id)
                        continue;
                    if (message.Side == MessageSide.Me)
                        bubble.TextBlockSendMessageState.Text = message.SendMessageState.ToString();
                    switch (message.MessageType)
                    {
                        case MessageType.Text:
                            var textView = (TextView)bubble.ContentView.Content;
                            if (textView != null)
                                textView.TextBlockPlainText.Text = message.ThreadText ?? "Invalid Message";
                            break;
                        case MessageType.Media:
                            var mediaView = (MediaView)bubble.ContentView.Content;
                            if (mediaView != null && message.ThreadMedia != null)
                                mediaView.PlainImage.ImageSource = (BitmapImage)await _photoImportService.ConvertPhotoBytesToPlatformImage(message.ThreadMedia);
                            break;
                        case MessageType.TextAndMedia:
                            var textAndMediaView = (TextAndMediaView)bubble.ContentView.Content;
                            if (textAndMediaView != null)
                            {
                                if (message.ThreadMedia != null)
                                    textAndMediaView.PlainImage.ImageSource = (BitmapImage)await _photoImportService.ConvertPhotoBytesToPlatformImage(message.ThreadMedia);
                                textAndMediaView.TextBlockPlainText.Text = message.ThreadText ?? "Invalid Message";
                            }
                            break;
                        default:
                            throw new Exception();
                    }

                    if (bubble.MessageID == lastItem.MessageID)
                        ScrollToLast(null, null);
                    break;
                }
            });

        }

        public async Task OnMessageEncrypted(Message message)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var lastItem = (BubbleBase)MessageThread.Items.LastOrDefault();
                if (lastItem == null)
                    return; // the thread was cleared in the meantime
                if (MessageThread.Items == null) return;
                foreach (var item in MessageThread.Items)
                {
                    var bubble = (BubbleBase)item;
                    if (bubble.MessageID != message.Id)
                        continue;

                    if (message.Side == MessageSide.Me)
                        bubble.TextBlockSendMessageState.Text = message.SendMessageState.ToString();

                    switch (message.MessageType)
                    {
                        case MessageType.Text:
                            var textView = (TextView)bubble.ContentView.Content;
                            if (textView != null)
                                textView.TextBlockCipherText.Text = VisualCrypt2Formatter.CreateVisualCryptText(message.TextCipher, -1).Text;
                            break;
                        case MessageType.Media:
                            var mediaView = (MediaView)bubble.ContentView.Content;
                            if (mediaView != null) mediaView.TextBlockCipherImage.Text = message.ImageCipherSnippet;
                            break;
                        case MessageType.TextAndMedia:
                            var textAndMediaView = (TextAndMediaView)bubble.ContentView.Content;
                            if (textAndMediaView != null)
                                textAndMediaView.TextBlockCipherImage.Text = message.ImageCipherSnippet;
                            break;
                        default:
                            throw new Exception();
                    }

                    if (bubble.MessageID == lastItem.MessageID)
                        ScrollToLast(null, null);
                    break;
                }
            });

        }

        public async Task OnMessageSending(Message message)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (MessageThread.Items != null)
                    foreach (var item in MessageThread.Items)
                    {
                        var bubble = (BubbleBase)item;
                        if (bubble.MessageID != message.Id)
                            continue;

                        bubble.TextBlockSendMessageState.Text = SendMessageState.Sending.ToString();
                        break;
                    }
            });
        }

        public async Task OnMessageErrorSending(Message message)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (MessageThread.Items != null)
                    foreach (var item in MessageThread.Items)
                    {
                        var bubble = (BubbleBase)item;
                        if (bubble.MessageID != message.Id)
                            continue;

                        bubble.TextBlockSendMessageState.Text = SendMessageState.ErrorSending.ToString();
                        break;
                    }
            });
        }

        public async Task OnSendAckReceived(Message message)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (MessageThread.Items == null)
                    return;
                foreach (var item in MessageThread.Items)
                {
                    var bubble = (BubbleBase)item;
                    if (bubble.MessageID != message.Id)
                        continue;
                    bubble.TextBlockSendMessageState.Text = SendMessageState.OnServer.ToString();
                    break;
                }
            });

        }

        public async Task OnReadReceiptReceived(Message message)
        {
            Debug.Assert(message.SendMessageState == SendMessageState.Read
                || message.SendMessageState == SendMessageState.Delivered);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (MessageThread.Items == null)
                    return;
                foreach (var item in MessageThread.Items)
                {
                    var bubble = (BubbleBase)item;
                    if (bubble.MessageID != message.Id)
                        continue;
                    bubble.TextBlockSendMessageState.Text = message.SendMessageState.ToString();
                    break;
                }
            });

        }

        async Task<BitmapImage> GetBitmapImageFromAbsolutePath(Message message)
        {
            var absoluteImagePath = Path.Combine(_fileService.GetLocalFolderPath(), message.ImageImportPath);
            var imageObject = await _fileService.LoadLocalImageAsync(absoluteImagePath);
            return (BitmapImage)imageObject;
        }

        void AddBubbleToThreadAndScrollToBottom(BubbleBase bubble, bool scrollToBottom)
        {
            _previousBubbleThatWasAddedAtBottom?.RemoveBottomBubbleStyles();
            bubble.AddBottomBubbleStyles();
            if (scrollToBottom)
                bubble.Loaded += ScrollToLast;
            MessageThread.Items?.Add(bubble);
            Debug.WriteLine($"MessageThread.Width: {MessageThread.ActualWidth}");
            Debug.WriteLine($"Window.Width: {Window.Current.Bounds.Width}");

            _previousBubbleThatWasAddedAtBottom = bubble;
        }

        async Task<BubbleBase> CreateBubbleFromMessage(Message message)
        {
            var bubble = ChooseTemplate(message);
            bubble.MessageID = message.Id;
            if (message.Side == MessageSide.Me)
                bubble.TextBlockSendMessageState.Text = message.SendMessageState.ToString();
            bubble.TextBlockDateString.Text = message.GetDateString();

            switch (message.MessageType)
            {
                case MessageType.Text:
                    var textView = new TextView();
                    if (message.TextCipher != null)
                        textView.TextBlockCipherText.Text = VisualCrypt2Formatter.CreateVisualCryptText(message.TextCipher, -1).Text;
                    textView.TextBlockPlainText.Text = message.ThreadText ?? string.Empty;
                    bubble.ContentView.Content = textView;
                    break;
                case MessageType.Media:
                    var mediaView = new MediaView();
                    mediaView.TextBlockCipherImage.Text = message.ImageCipherSnippet ?? string.Empty;
                    if (!string.IsNullOrEmpty(message.ImageImportPath))
                        mediaView.PlainImage.ImageSource = await GetBitmapImageFromAbsolutePath(message);
                    bubble.ContentView.Content = mediaView;
                    break;
                case MessageType.TextAndMedia:
                    var textAndMediaView = new TextAndMediaView();
                    textAndMediaView.TextBlockPlainText.Text = message.ThreadText ?? string.Empty;
                    textAndMediaView.TextBlockCipherImage.Text = message.ImageCipherSnippet ?? string.Empty;
                    if (!string.IsNullOrEmpty(message.ImageImportPath))
                        textAndMediaView.PlainImage.ImageSource = await GetBitmapImageFromAbsolutePath(message);
                    bubble.ContentView.Content = textAndMediaView;
                    break;
                default:
                    throw new Exception("Invalid MessageType.");
            }




            return bubble;
        }

        void ScrollToLast(object o, RoutedEventArgs args)
        {
            MessageThread.UpdateLayout();
            ScrollViewer.ChangeView(null, ScrollViewer.ScrollableHeight, null, false);
        }

        BubbleBase ChooseTemplate(Message message)
        {
            if (message.Side == MessageSide.Me)
            {
                if (message.PrevSide == MessageSide.You)
                    return new MeUnderYou();
                return new MeUnderMe();
            }
            if (message.PrevSide == MessageSide.You)
                return new YouUnderYou();
            return new YouUnderMe();
        }


        async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await _messagesViewModel.OnViewDidLoad();
        }

        void OnUnloaded(object sender, RoutedEventArgs e)
        {

        }

        async void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs ea)
        {
            if (_isBusy)
            {
                Debug.WriteLine("Busy, won't do more work!");
                return;
            }
            if (ea.IsIntermediate)
            {
                return;
            }
            var percent = ScrollViewer.VerticalOffset / ScrollViewer.ScrollableHeight;
            var scrollableHeight = ScrollViewer.ScrollableHeight;
            var verticalOffset = ScrollViewer.VerticalOffset;
            Debug.WriteLine($"Offset: {percent} | ScollableHeight: {scrollableHeight} | VerticalOffset: {verticalOffset}");
            if (percent > 0.05)
            {
                Debug.WriteLine($"Not at Top, still at {percent.ToString("F1")} %");
                return;
            }


            ProgressBar.Visibility = Visibility.Visible;
            _isBusy = true;
            try
            {
                var heightBefore = ScrollViewer.ScrollableHeight;
                Debug.WriteLine($"BEFORE ScollableHeight: {ScrollViewer.ScrollableHeight} | VerticalOffset: {ScrollViewer.VerticalOffset}");
                var messages = await _messagesViewModel.OnViewMoreMessagesRequestedAsync();
                var reversed = messages.Reverse();
                foreach (var message in reversed)
                {
                    BubbleBase bubble = await CreateBubbleFromMessage(message);
                    MessageThread.Items?.Insert(0, bubble);
                }
                MessageThread.UpdateLayout();
                var heightAfter = ScrollViewer.ScrollableHeight;
                Debug.WriteLine($"AFTER ScollableHeight: {ScrollViewer.ScrollableHeight} | VerticalOffset: {ScrollViewer.VerticalOffset}");
                ScrollViewer.ChangeView(null, heightAfter - heightBefore, null, true);

            }
            catch (Exception)
            {
                _isBusy = false;
                ProgressBar.Visibility = Visibility.Collapsed;
            }
            _isBusy = false;
            ProgressBar.Visibility = Visibility.Collapsed;
        }


    }
}