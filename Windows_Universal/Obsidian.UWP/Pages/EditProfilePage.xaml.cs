using System.Diagnostics;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Obsidian.UWP.Pages
{
    public sealed partial class EditProfilePage
    {
        public EditProfilePage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
           
        }
        void OnLoaded(object sender, RoutedEventArgs e)
        {

            if (ShouldGoToWideState())
            {
                // We shouldn't see this page since we are in "wide master-detail" mode.
                // Play a transition as we are navigating from a separate page.
                NavigateBackForWideState(useTransition: true);
            }
            else
            {
                // Realize the main page content.
                FindName("RootPanel");
            }

            Window.Current.SizeChanged += OnWindowSizeChanged;

            Debug.WriteLine("EditProfilePage OnLoaded()");
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // RegisterType for hardware and software back request from the system
            SystemNavigationManager systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.BackRequested += OnBackRequested;
            systemNavigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

            Debug.WriteLine("Navigated to EditProfilePage.");
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            SystemNavigationManager systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.BackRequested -= OnBackRequested;
            systemNavigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }
        void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            // Mark event as handled so we don't get bounced out of the app.
            e.Handled = true;
            // Page above us will be our master view.
            // Make sure we are using the "drill out" animation in this transition.
            Frame.Navigate(typeof(ChatMasterPage), "Back", new EntranceNavigationTransitionInfo());
        }
        void NavigateBackForWideState(bool useTransition)
        {
            // Evict this page from the cache as we may not need it again.
            NavigationCacheMode = NavigationCacheMode.Disabled;

            if (useTransition)
            {
                Frame.GoBack(new EntranceNavigationTransitionInfo());
            }
            else
            {
                Frame.GoBack(new SuppressNavigationTransitionInfo());
            }
        }
        bool ShouldGoToWideState()
        {
            return Window.Current.Bounds.Width >= 720;
        }
        void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged -= OnWindowSizeChanged;
        }
        void OnWindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            if (ShouldGoToWideState())
            {
                // Make sure we are no longer listening to window change events.
                Window.Current.SizeChanged -= OnWindowSizeChanged;

                // We shouldn't see this page since we are in "wide master-detail" mode.
                NavigateBackForWideState(useTransition: false);
            }
        }
        void DeleteItem(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ChatMasterPage), "Delete", new EntranceNavigationTransitionInfo());
        }
    }
}
