using Windows.UI.Xaml;
using Obsidian.Applications.ViewModels.Chat;
using System;

namespace Obsidian.UWP.Controls.Chat.Messages
{
    public sealed partial class MessagesUserControl : IMessagesView
    {
	    public MessagesUserControl()
        {
	        InitializeComponent();
			Hide();
			var messagesViewModel = Application.Current.GetContainer().Get<MessagesViewModel>();
	        messagesViewModel.MessagesView = this;
		}

		public void Hide()
		{
			this.MainGrid.Visibility = Visibility.Collapsed;
			this.WelcomeGrid.Visibility = Visibility.Visible;
		}

		public void Show()
		{
			this.MainGrid.Visibility = Visibility.Visible;
			this.WelcomeGrid.Visibility = Visibility.Collapsed;
		}
	}
}
