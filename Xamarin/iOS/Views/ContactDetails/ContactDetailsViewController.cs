using System;
using GalaSoft.MvvmLight.Helpers;
using GalaSoft.MvvmLight.Ioc;
using ObsidianMobile.Core.Interfaces.Models;
using ObsidianMobile.Core.Interfaces.ViewModels;
using ObsidianMobile.iOS.Extensions;
using UIKit;

namespace ObsidianMobile.iOS.Views
{
    public partial class ContactDetailsViewController : BaseViewController
    {
        UIBarButtonItem _doneButton;

        IContactDetailsViewModel ViewModel
        {
            get
            {
                return (IContactDetailsViewModel)_viewModel;
            }
        }

        public ContactDetailsViewController() : base("ContactDetailsViewController", null)
        {
            Initialize();

            _doneButton.SetCommand(ViewModel.CreateContactCommand);
        }

        public ContactDetailsViewController(IContact contact) : base("ContactDetailsViewController", null)
        {
            Initialize(contact);

            _doneButton.SetCommand(ViewModel.UpdateContactCommand, contact);
        }

        private void Initialize(IContact contact = null)
        {
            _viewModel = SimpleIoc.Default.GetInstance<IContactDetailsViewModel>();
            _viewModel.OnStart();

            ViewModel.ContactName = contact?.Name ?? String.Empty;

            _doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done);
            _doneButton.TintColor = UIColor.FromRGB(34, 209, 194);
            NavigationItem.RightBarButtonItem = _doneButton;
        }

        protected override void SetBindings()
        {
            base.SetBindings();

            _bindings.Add(this.SetBinding(() => ViewModel.ContactName, () => FirstNameTextField.Text, BindingMode.TwoWay));
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            FirstNameTextField.BecomeFirstResponder();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ProfileImageView.MakeRoundImageView();
            View.BackgroundColor = UIColor.FromRGB(190, 234, 232);

            _bindings.Add(this.SetBinding(() => ViewModel.ContactName, () => FirstNameTextField.Text, BindingMode.TwoWay));
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            FirstNameTextField.EndEditing(true);
        }
    }
}

