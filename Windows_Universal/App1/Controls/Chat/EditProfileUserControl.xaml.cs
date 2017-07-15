using Windows.UI.Xaml;
using Obsidian.Applications.Data;
using Obsidian.Applications.Services.Interfaces;
using Obsidian.Applications.ViewModels.Chat;
using Obsidian.Cryptography.Api.Interfaces;

namespace Obsidian.UWP.Controls.Chat
{
    public sealed partial class EditProfileUserControl
    {
        readonly ProfileViewModel _profileViewModel;
        readonly AppRepository _appRepository;
        readonly IPhotoImportService _photoImportService;
        readonly IVisualCrypt2Service _visualCrypt2Service;
        public EditProfileUserControl()
        {
            InitializeComponent();
            var container = Application.Current.GetContainer();
            _profileViewModel = container.Get<ProfileViewModel>();
            _appRepository = container.Get<AppRepository>();
            _photoImportService = container.Get<IPhotoImportService>();
            _visualCrypt2Service = container.Get<IVisualCrypt2Service>();

            Loaded += OnLoaded;
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {

        }

        async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            byte[] photoBytes = await _photoImportService.GetProfilePhotoBytes();
            if (photoBytes == null)
                return;

            var copy = new byte[photoBytes.Length];
            photoBytes.CopyTo(copy,0);
            var encryptedbytes = _visualCrypt2Service.DefaultEncrypt(photoBytes);
            if (encryptedbytes == null)
                return;
            await _appRepository.UpdateEncryptedProfileImage(_profileViewModel.Profile.Id, encryptedbytes);

            // photoBytes is now zeroed, that's why we need the copy!!!
            _profileViewModel.ProfileImage = await _photoImportService.ConvertPhotoBytesToPlatformImage(copy);
        }


    }
}
