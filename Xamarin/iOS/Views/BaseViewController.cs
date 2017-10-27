using System.Collections.Generic;
using GalaSoft.MvvmLight.Helpers;
using GalaSoft.MvvmLight.Ioc;
using ObsidianMobile.Core.Interfaces.Navigation;
using ObsidianMobile.Core.Interfaces.ViewModels;
using UIKit;

namespace ObsidianMobile.iOS.Views
{
    public class BaseViewController : UIViewController
    {
        protected List<Binding> _bindings = new List<Binding>();
        protected IBaseViewModel _viewModel;

        IObsidianNavigationService _navigator;

        public BaseViewController()
        {
            Initialize();
        }

        public BaseViewController(string nibName, Foundation.NSBundle bundle) : base(nibName, bundle)
        {
            Initialize();
        }

        private void Initialize()
        {
            _navigator = SimpleIoc.Default.GetInstance<IObsidianNavigationService>();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();


            SetBindings();
        }

        protected virtual void SetBindings()
        {

        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(21,94,91);

            if (_navigator.CanGoBack)
            {
                this.NavigationItem.HidesBackButton = true;
                var backButton = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, (sender, e) => _navigator.GoBack());
                backButton.TintColor = UIColor.FromRGB(34,209,194);
                this.NavigationItem.LeftBarButtonItem = backButton;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _bindings.Clear();
            _viewModel = null;
        }
    }
}