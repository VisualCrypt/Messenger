using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Obsidian.Applications.ViewModels.Chat;

namespace Obsidian.UWP.Controls.Chat.Messages
{
    public sealed partial class MessageThreadHeaderUserControl
    {
        readonly ContactsViewModel _contactsViewModel;
        readonly ProfileViewModel _profileViewModel;
        public MessageThreadHeaderUserControl()
        {
            InitializeComponent();
            _contactsViewModel = Application.Current.GetContainer().Get<ContactsViewModel>();
            _profileViewModel = Application.Current.GetContainer().Get<ProfileViewModel>();
        }

        void ButtonCall_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            ((Button) sender).IsEnabled = !((Button) sender).IsEnabled;
        }
    }
}
