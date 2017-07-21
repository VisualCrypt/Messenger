using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Obsidian.Applications.ViewModels.Chat;

namespace Obsidian.UWP.Controls.Chat
{
    public sealed partial class AddContactUserControl
    {
        readonly ContactsViewModel _contactsViewModel;
        public AddContactUserControl()
        {
            InitializeComponent();
            _contactsViewModel = Application.Current.GetContainer().Get<ContactsViewModel>();
            TextBoxContactID.TextChanged += TextBoxContactIDTextChanged;
            GoToNormalState();
        }

        void GoToNormalState()
        {
            TextBoxContactID.Text = string.Empty;
            RowTextBox.Height = new GridLength(0);
            TextBlockTitle.Text = "Contacts";
            BtnAddContact.Visibility = Visibility.Visible;
            BtnSaveAddedContact.Visibility = Visibility.Collapsed;
            BtnSelectContact.Visibility = Visibility.Visible;
            BtnCancelAll.Visibility = Visibility.Collapsed;
            BtnDeleteContact.Visibility = Visibility.Collapsed;
            BtnConfirmDeleteContact.Visibility = Visibility.Collapsed;
            PanelAddContact.Visibility = Visibility.Collapsed;
            PanelDeleteContact.Visibility = Visibility.Collapsed;
        }
        void TextBoxContactIDTextChanged(object sender, TextChangedEventArgs e)
        {
            _contactsViewModel.AddedContactID = TextBoxContactID.Text;
        }

        void OnCancelAllClick(object sender, RoutedEventArgs e)
        {
            GoToNormalState();
        }

        void AddContactClick(object sender, RoutedEventArgs e)
        {
            RowTextBox.Height = GridLength.Auto;
            TextBoxContactID.Focus(FocusState.Programmatic);
            TextBlockTitle.Text = "Add Contact";

            BtnAddContact.Visibility = Visibility.Collapsed;
            BtnSaveAddedContact.Visibility = Visibility.Visible;
            BtnSelectContact.Visibility = Visibility.Collapsed;
            BtnCancelAll.Visibility = Visibility.Visible;
            BtnDeleteContact.Visibility = Visibility.Collapsed;
            BtnConfirmDeleteContact.Visibility = Visibility.Collapsed;
            PanelAddContact.Visibility = Visibility.Visible;
	        _contactsViewModel.CurrentError = "";
        }

        void DeleteContactClick(object sender, RoutedEventArgs e)
        {
            RowTextBox.Height = GridLength.Auto;
            TextBoxContactID.Focus(FocusState.Programmatic);
            TextBlockTitle.Text = "Delete Contact(s)";

            BtnAddContact.Visibility = Visibility.Collapsed;
            BtnSaveAddedContact.Visibility = Visibility.Collapsed;
            BtnSelectContact.Visibility = Visibility.Collapsed;
            BtnCancelAll.Visibility = Visibility.Visible;
            BtnDeleteContact.Visibility = Visibility.Collapsed;
            BtnConfirmDeleteContact.Visibility = Visibility.Visible;
            PanelDeleteContact.Visibility = Visibility.Visible;
        }

        void SelectContactsClick(object sender, RoutedEventArgs e)
        {
            TextBlockTitle.Text = "Edit or Delete Contacts";

            BtnAddContact.Visibility = Visibility.Collapsed;
            BtnSaveAddedContact.Visibility = Visibility.Collapsed;
            BtnSelectContact.Visibility = Visibility.Collapsed;
            BtnCancelAll.Visibility = Visibility.Visible;
            BtnDeleteContact.Visibility = Visibility.Visible;
        }

      
    }
}
