using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Obsidian.Applications.Models.Chat;
using Obsidian.Applications.ViewModels.Chat;

namespace Obsidian.UWP.Pages
{
    public sealed partial class ChatMasterPage
    {
        readonly ContactsViewModel _contactsViewModel;
        readonly MessagesViewModel _messagesViewModel;

        string Title => "Chat";

        public ChatMasterPage()
        {
            InitializeComponent();
            _contactsViewModel = Application.Current.GetContainer().Get<ContactsViewModel>();
           
            _messagesViewModel = Application.Current.GetContainer().Get<MessagesViewModel>();
            ContactsUserControl.AddContactUserControl.BtnSelectContact.Click += SelectItems;
            ContactsUserControl.AddContactUserControl.BtnCancelAll.Click += CancelSelection;
            ContactsUserControl.AddContactUserControl.BtnConfirmDeleteContact.Click += CancelSelection;
			ContactsUserControl.
            Loaded += OnLoaded;
        }

        async void RunWithDispatcher(Action callback)
        {
            var action = callback;
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    action();
                });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Details view can remove an item from the list.
            if (e.Parameter as string == "Delete")
            {
                DeleteItem(null, null);
            }
            EditProfileUserControl.Visibility = Visibility.Collapsed;
	        EditContactUserControl.Visibility = Visibility.Collapsed;
			MessagesUserControl.Visibility = Visibility.Visible;
            CompositionUserControl.Visibility = Visibility.Visible;
            base.OnNavigatedTo(e);
            Debug.WriteLine("Navigated to ChatMasterPage.");
        }

        //protected override void OnNavigatedFrom(NavigationEventArgs e)
        //{
        //    base.OnNavigatedFrom(e);
        //    SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;
        //}

        //ListView MasterListView { get { return ContactsUserControl.MasterListView; } }
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            
            if (VisualStatesGroup_PageSize.CurrentState == NarrowState)
            {
                VisualStateGroup_PageSize_NarrowState_Setters();

                VisualStateManager.GoToState(this, MasterState.Name, true);
                VisualStateGroup_MasterDetails_MasterState_Setters();
            }
            else if (VisualStatesGroup_PageSize.CurrentState == WideState)
            {
                VisualStateGroup_PageSize_WideState_Setters();
               
                VisualStateManager.GoToState(this, MasterDetailsState.Name, true);
                VisualStateGroup_MasterDetails_MasterDetailState_Setters();
            }
            else
            {

                try
                {
                    throw new InvalidOperationException();
                }
                catch { }
            }

            //SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;
            //SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

            ContactsUserControl.MasterListView.SelectionChanged += OnSelectionChanged;
            ContactsUserControl.MasterListView.ItemClick += OnItemClick;
	        ContactsUserControl.EditContactClicked = OnEditContactClicked;

        }

        void VisualStateGroup_PageSize_WideState_Setters()
        {
            IdentityUserControl.Visibility = Visibility.Visible;
            ContactsUserControl.Visibility = Visibility.Visible;
            MessagesUserControl.Visibility = Visibility.Visible;
            CompositionUserControl.Visibility = Visibility.Visible;
            MasterColumn.Width = new GridLength(320);
            DetailColumn.Width = new GridLength(1,GridUnitType.Star);
            ContactsUserControl.MasterListView.SelectedItem = _contactsViewModel.CurrentContact;
        }
        void VisualStateGroup_PageSize_NarrowState_Setters()
        {
            IdentityUserControl.Visibility = Visibility.Visible;
            ContactsUserControl.Visibility = Visibility.Visible;
            MessagesUserControl.Visibility = Visibility.Collapsed;
            CompositionUserControl.Visibility = Visibility.Collapsed;
            MasterColumn.Width = new GridLength(1, GridUnitType.Star);
            DetailColumn.Width = new GridLength(0);
        }

        void VisualStateGroup_MasterDetails_MasterState_Setters()
        {
            ContactsUserControl.MasterListView.SelectionMode = ListViewSelectionMode.None;
            ContactsUserControl.MasterListView.IsItemClickEnabled = true;
            // Page AppBar Commands in XAML!
        }
        void VisualStateGroup_MasterDetails_MasterDetailState_Setters()
        {
            ContactsUserControl.MasterListView.SelectionMode = ListViewSelectionMode.Extended;
            ContactsUserControl.MasterListView.IsItemClickEnabled = false;
            // Page AppBar Commands in XAML!
        }
        void VisualStateGroup_MasterDetails_ExtendedSelectionState_Setters()
        {
            // nothing!
            // Page AppBar Commands in XAML!
        }
        void VisualStateGroup_MasterDetails_MultipleSelectionState_Setters()
        {
            ContactsUserControl.MasterListView.SelectionMode = ListViewSelectionMode.Multiple;
            ContactsUserControl.MasterListView.IsItemClickEnabled = false;
            // nothing!
            // Page AppBar Commands in XAML!
        }
        void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
           
            bool isNarrow = e.NewState == NarrowState;
            if (isNarrow)
            {
                VisualStateGroup_PageSize_NarrowState_Setters();
                //Frame.Navigate(typeof(ChatSessionPage), new SuppressNavigationTransitionInfo());
            }
            else
            {
                VisualStateGroup_PageSize_WideState_Setters();
                VisualStateManager.GoToState(this, MasterDetailsState.Name, true);
                VisualStateGroup_MasterDetails_MasterDetailState_Setters();

                ContactsUserControl.MasterListView.SelectedItem = _contactsViewModel.CurrentContact;
            }

            EntranceNavigationTransitionInfo.SetIsTargetElement(ContactsUserControl.MasterListView, isNarrow);
            if (MessagesUserControl != null)
            {
                EntranceNavigationTransitionInfo.SetIsTargetElement(MessagesUserControl, !isNarrow);
            }
        }
        void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VisualStatesGroup_PageSize.CurrentState == WideState)
            {
                if (ContactsUserControl.MasterListView.SelectedItems.Count == 1)
                {
                    _contactsViewModel.CurrentContact = ContactsUserControl.MasterListView.SelectedItem as Identity;
                    MessagesUserControl.Transitions.Clear();
                    MessagesUserControl.Transitions.Add(new EntranceThemeTransition());
                    EditProfileUserControl.Visibility = Visibility.Collapsed;
	                EditContactUserControl.Visibility = Visibility.Collapsed;
                    MessagesUserControl.Visibility = Visibility.Visible;
                    CompositionUserControl.Visibility = Visibility.Visible;
                }
                // Entering in Extended selection
                else if (ContactsUserControl.MasterListView.SelectedItems.Count > 1
                     && MasterDetailsStatesGroup.CurrentState == MasterDetailsState)
                {
                    VisualStateManager.GoToState(this, ExtendedSelectionState.Name, true);
                    VisualStateGroup_MasterDetails_ExtendedSelectionState_Setters();
                }
            }
	        if (VisualStatesGroup_PageSize.CurrentState == NarrowState)
	        {
				if (ContactsUserControl.MasterListView.SelectedItems.Count == 1)
				{
					_contactsViewModel.CurrentContact = ContactsUserControl.MasterListView.SelectedItem as Identity;
					Frame.Navigate(typeof(ChatSessionPage), new DrillInNavigationTransitionInfo());
				}
			}
            // Exiting Extended selection
            if (MasterDetailsStatesGroup.CurrentState == ExtendedSelectionState &&
                ContactsUserControl.MasterListView.SelectedItems.Count == 1)
            {
                VisualStateManager.GoToState(this, MasterDetailsState.Name, true);
                VisualStateGroup_MasterDetails_MasterDetailState_Setters();
            }
        }
        // ItemClick event only happens when user is a Master state. In this state, 
        // selection mode is none and click event navigates to the details view.
        void OnItemClick(object sender, ItemClickEventArgs e)
        {
            // The clicked item it is the new selected contact
            _contactsViewModel.CurrentContact = e.ClickedItem as Identity;
            if (VisualStatesGroup_PageSize.CurrentState == NarrowState)
            {
                // Go to the details page and display the item 
                Frame.Navigate(typeof(ChatSessionPage), new DrillInNavigationTransitionInfo());
            }
            else
            {
                // Play a refresh animation when the user switches detail items.
                MessagesUserControl.Transitions.Clear();
                MessagesUserControl.Transitions.Add(new EntranceThemeTransition());
              
            }
        }

	    void OnEditContactClicked(Identity identity)
	    {
			if (VisualStatesGroup_PageSize.CurrentState == NarrowState)
			{ 
				Frame.Navigate(typeof(EditContactPage), identity, new DrillInNavigationTransitionInfo());
			}
			else
			{
				_contactsViewModel.ContactToEdit = identity;
				EditContactUserControl.Transitions.Clear();
				EditContactUserControl.Transitions.Add(new EntranceThemeTransition());
				EditContactUserControl.Visibility = Visibility.Visible;
				EditContactUserControl.StartEditing();

				ContactsUserControl.MasterListView.SelectedItem = null; // so that we can use onselectionchanged to reset
				EditProfileUserControl.Visibility = Visibility.Collapsed;
				MessagesUserControl.Visibility = Visibility.Collapsed;
				CompositionUserControl.Visibility = Visibility.Collapsed;

			}
		}
        void IdentityUserControl_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            if (VisualStatesGroup_PageSize.CurrentState == NarrowState)
            {
                Frame.Navigate(typeof(EditProfilePage), new DrillInNavigationTransitionInfo());
            }
            else
            {

                EditProfileUserControl.Transitions.Clear();
                EditProfileUserControl.Transitions.Add(new EntranceThemeTransition());
                ContactsUserControl.MasterListView.SelectedItem = null; // so that we can use onselectionchanged to reset
                EditProfileUserControl.Visibility = Visibility.Visible;
				EditContactUserControl.Visibility = Visibility.Collapsed;
                MessagesUserControl.Visibility = Visibility.Collapsed;
                CompositionUserControl.Visibility = Visibility.Collapsed;

            }
        }

      
        #region Commands
        void OnAddContactClick(object sender, RoutedEventArgs e)
        {
            _contactsViewModel.Contacts.Add(new Identity() { Id = "Test"});

            // Select this item in case that the list is empty
            if (ContactsUserControl.MasterListView.SelectedIndex == -1)
            {
                ContactsUserControl.MasterListView.SelectedIndex = 0;
                _contactsViewModel.CurrentContact = ContactsUserControl.MasterListView.SelectedItem as Identity;
                // Details view is collapsed, in case there is not items.
                // You should show it just in case. 
                MessagesUserControl.Visibility = Visibility.Visible;
            }
        }
        void DeleteItem(object sender, RoutedEventArgs e)
        {
            if (_contactsViewModel.CurrentContact != null)
            {
                _contactsViewModel.Contacts.Remove(_contactsViewModel.CurrentContact);

                if (ContactsUserControl.MasterListView.Items != null && ContactsUserControl.MasterListView.Items.Count > 0)
                {
                    ContactsUserControl.MasterListView.SelectedIndex = 0;
                    _contactsViewModel.CurrentContact = ContactsUserControl.MasterListView.SelectedItem as Identity;
                }
                else
                {
                    // Details view is collapsed, in case there is not items.
                    MessagesUserControl.Visibility = Visibility.Collapsed;
                    _contactsViewModel.CurrentContact = null;
                }
            }
        }
        void DeleteItems(object sender, RoutedEventArgs e)
        {
            if (ContactsUserControl.MasterListView.SelectedIndex != -1)
            {
                List<Identity> selectedItems = new List<Identity>();
                foreach (Identity contact in ContactsUserControl.MasterListView.SelectedItems)
                {
                    selectedItems.Add(contact);
                }
                foreach (Identity contact in selectedItems)
                {
                    _contactsViewModel.Contacts.Remove(contact);
                }
                if (ContactsUserControl.MasterListView.Items != null && ContactsUserControl.MasterListView.Items.Count > 0)
                {
                    ContactsUserControl.MasterListView.SelectedIndex = 0;
                    _contactsViewModel.CurrentContact = ContactsUserControl.MasterListView.SelectedItem as Identity;
                }
                else
                {
                    MessagesUserControl.Visibility = Visibility.Collapsed;
                }
            }
        }
        void SelectItems(object sender, RoutedEventArgs e)
        {
            if (ContactsUserControl.MasterListView.Items != null && ContactsUserControl.MasterListView.Items.Count > 0)
            {
                VisualStateManager.GoToState(this, MultipleSelectionState.Name, true);
                VisualStateGroup_MasterDetails_MultipleSelectionState_Setters();
            }
        }
        void CancelSelection(object sender, RoutedEventArgs e)
        {
            // Unsubscribe, so that the captured selection does not get overwritten
            ContactsUserControl.MasterListView.SelectionChanged -=
                ContactsUserControl.CaptureSelectedContacts;

            if (VisualStatesGroup_PageSize.CurrentState == NarrowState)
            {
                VisualStateManager.GoToState(this, MasterState.Name, true);
                VisualStateGroup_MasterDetails_MasterState_Setters();
            }
            else
            {
                VisualStateManager.GoToState(this, MasterDetailsState.Name, true);
                VisualStateGroup_MasterDetails_MasterDetailState_Setters();
            }
        }

        #endregion

        //void AppBarButton_Back_Click(object sender, RoutedEventArgs e)
        //{
        //    Frame.Navigate(typeof(FilesPage), new EntranceNavigationTransitionInfo());
        //}

        //void OnBackRequested(object sender, BackRequestedEventArgs e)
        //{
        //    e.Handled = true;
        //    Frame.Navigate(typeof(FilesPage), new EntranceNavigationTransitionInfo());

        //}

       
    }
}
