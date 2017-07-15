using Windows.UI.Xaml;
using Obsidian.Applications.ViewModels.Chat;

namespace Obsidian.UWP.Controls.Chat
{
    public sealed partial class ProfileUserControl
    {
        readonly ProfileViewModel _profileViewModel;

        public ProfileUserControl()
        {
            InitializeComponent();
            _profileViewModel = Application.Current.GetContainer().Get<ProfileViewModel>();
            Loaded += OnLoaded;
        }

        void OnLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _profileViewModel.OnViewDidLoad();
        }

       
    }
}
