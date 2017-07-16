using System;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Obsidian.Applications.Services.Interfaces;
using Obsidian.Applications.ViewModels.Chat;

namespace Obsidian.UWP.Controls.Chat.Messages
{
    public sealed partial class CompositionUserControl : ICompositionView
    {
        readonly SendMessagesController _sendMessagesController;
        readonly IPhotoImportService _photoImportService;

        public CompositionUserControl()
        {
            InitializeComponent();
			Hide();
            var container = Application.Current.GetContainer();
            _sendMessagesController = container.Get<SendMessagesController>();
            _photoImportService = container.Get<IPhotoImportService>();
	        var messagesViewModel = Application.Current.GetContainer().Get<MessagesViewModel>();
	        messagesViewModel.CompositionView = this;
			Loaded += OnLoaded;
            if (!App.IsPhone())
            {
                CompositionBox.AcceptsReturn = false;
                CompositionBox.KeyDown += TweakEnterKeyBehavior;
            }
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            CompositionBox.TextChanged += OnCompositionBoxOnTextChanged;
            CompositionBox.Text = string.Empty;
        }

        void OnCompositionBoxOnTextChanged(object sender, TextChangedEventArgs e)
        {
            ButtonSend.IsEnabled = !string.IsNullOrWhiteSpace(CompositionBox.Text);

        }

	    public void Hide()
	    {
		    this.MainGrid.Visibility = Visibility.Collapsed;
	    }

	    public void Show()
	    {
		    this.MainGrid.Visibility = Visibility.Visible;
	    }

		void TweakEnterKeyBehavior(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key.Equals(VirtualKey.Enter))
            {
                e.Handled = true;
                var state2 = CoreWindow.GetForCurrentThread().GetAsyncKeyState(VirtualKey.Shift);
                if (state2 != CoreVirtualKeyStates.Down)
                {
                    if (!string.IsNullOrWhiteSpace(CompositionBox.Text))
                        OnButtonSendTapped(null, null);
                }
                else
                {
                    CompositionBox.Text += Environment.NewLine;
                    CompositionBox.Select(CompositionBox.Text.Length, 0);
                }
            }
        }

        async void OnButtonSendTapped(object sender, RoutedEventArgs e)
        {
            if (!ButtonSend.IsEnabled)
                return;
            await _sendMessagesController.SendMessage(CompositionBox.Text, null);
            CompositionBox.Text = string.Empty;
        }

        async void OnButtonAddMediaTapped(object sender, TappedRoutedEventArgs e)
        {
            await SendMessage();
        }

        async Task SendMessage()
        {
            try
            {
                string importPhotoFutureAccessPath = await _photoImportService.GetPhotoFutureAccessPath();

                if (importPhotoFutureAccessPath == null)
                    return;
                var messageText = CompositionBox.Text;
                CompositionBox.Text = string.Empty;
                await Task.Run(() => _sendMessagesController.SendMessage(messageText, importPhotoFutureAccessPath))
                    .ConfigureAwait(false); // http://blog.stephencleary.com/2012/07/dont-block-on-async-code.html
                // ConfigureAwait(false) is very important here, to enable downstream locking while sending.
            }
            catch (Exception e)
            {
                Application.Current.GetContainer().Get<ILog>()?.Exception(e);
                var mbsvc = Application.Current.GetContainer().Get<IMessageBoxService>();
                if (mbsvc != null)
                    await mbsvc.ShowError(e.Message);
            }
        }


    }
}
