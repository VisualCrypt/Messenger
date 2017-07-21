using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Obsidian.Applications.Models.Chat;
using Obsidian.Applications.ViewModels.Chat;

namespace Obsidian.UWP.Controls.Chat
{
    public sealed partial class ContactsUserControl
    {
		// Will be set from ChatMasterPage
	    public Action<Identity> EditContactClicked;

        readonly ContactsViewModel _contactsViewModel;

        public ContactsUserControl()
        {
            InitializeComponent();
            _contactsViewModel = Application.Current.GetContainer().Get<ContactsViewModel>();
            Loaded += OnLoaded;

            AddContactUserControl.BtnSelectContact.Click += BtnSelectContact_Click;
        }

        void BtnSelectContact_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            MasterListView.SelectionChanged -= CaptureSelectedContacts;
            MasterListView.SelectionChanged += CaptureSelectedContacts;
        }

        public void CaptureSelectedContacts(object sender, SelectionChangedEventArgs e)
        {
            var selectedItems = MasterListView.SelectedItems ?? new List<object>();
            _contactsViewModel.SelectedContacts.Clear();
            var sb = new StringBuilder();
            foreach (Identity identity in selectedItems)
            {
                _contactsViewModel.SelectedContacts.Add(identity);
                sb.Append($"@{identity.Id}, ");
            }
            var deleteList = sb.ToString();
            if (deleteList.Length >= 2)
                _contactsViewModel.DeleteList = deleteList.Remove(deleteList.Length - 2);
        }

        void OnLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (_contactsViewModel.CurrentContact == null && _contactsViewModel.Contacts.Count > 0)
            {
                _contactsViewModel.CurrentContact = _contactsViewModel.Contacts[0];
                MasterListView.SelectedIndex = 0;
            }
            _contactsViewModel.OnViewDidLoad();
        }

	    void BtnEditContact_OnClick(object sender, RoutedEventArgs e)
	    {
		    var button = sender as Button;
		    var identity = button?.DataContext as Identity;
			if(identity == null) return;
		    EditContactClicked?.Invoke(identity);
	    }
    }
}
