using UIKit;
using GalaSoft.MvvmLight.Ioc;
using ObsidianMobile.Core.Interfaces.ViewModels;
using GalaSoft.MvvmLight.Helpers;

namespace ObsidianMobile.iOS.Views
{
    public class ContactsListController : BaseViewController
    {
        IContactListViewModel ViewModel
        {
            get
            {
                return (IContactListViewModel)_viewModel;
            }
        }

        public UITableView ContactsTableView { get; set; }

        public ContactsListController()
        {
            _viewModel = SimpleIoc.Default.GetInstance<IContactListViewModel>();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Initialize();
        }

        private void Initialize()
        {
            ContactsTableView = new UITableView()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                AllowsMultipleSelection = false,
                BackgroundColor = UIColor.FromRGB(190,234,232)
            };

            ViewModel.UpdateContacts();

            var source = ViewModel.Contacts.GetTableViewSource((arg1, arg2, arg3) => { }, null, () => new ContactsTableViewSource());

            (source as ContactsTableViewSource).OnRowSelected = (item) =>
            {
                ContactsTableView.DeselectRow(ContactsTableView.IndexPathForSelectedRow, true);
                ViewModel.NavigateToChatCommand.Execute(item);
            };

            ContactsTableView.RegisterNibForCellReuse(ContactCell.Nib, ContactCell.Key);
            ContactsTableView.Source = source;

            View.AddSubviews(ContactsTableView);

            var addContact = new UIBarButtonItem(UIBarButtonSystemItem.Add, (sender, e) =>
            {
                ViewModel.NavigateToContactDetailsCommand.Execute(null);
            });

            addContact.TintColor = UIColor.FromRGB(34, 209, 194);

            NavigationItem.RightBarButtonItem = addContact;

            InitializeConstraints();
        }

        private void InitializeConstraints()
        {
            var constraints = new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(ContactsTableView,NSLayoutAttribute.Leading, NSLayoutRelation.Equal,View,NSLayoutAttribute.Leading,1,0),
                NSLayoutConstraint.Create(View,NSLayoutAttribute.Trailing, NSLayoutRelation.Equal,ContactsTableView,NSLayoutAttribute.Trailing,1,0),
                NSLayoutConstraint.Create(View,NSLayoutAttribute.Top, NSLayoutRelation.Equal,ContactsTableView,NSLayoutAttribute.Top,1,0),
                NSLayoutConstraint.Create(ContactsTableView,NSLayoutAttribute.Bottom, NSLayoutRelation.Equal,View,NSLayoutAttribute.Bottom,1,0),
            };
            View.AddConstraints(constraints);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            //TODO need to find better way
            ContactsTableView.ReloadData();
        }
    }
}
