using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Obsidian.Applications.Data;
using Obsidian.Applications.Infrastructure;
using Obsidian.Applications.Models.Chat;
using Obsidian.Applications.Services.Interfaces;
using Obsidian.Applications.Services.PortableImplementations;
using Obsidian.Applications.Workers;
using Obsidian.Cryptography.Api.Interfaces;
using Obsidian.Cryptography.E2E;
using Obsidian.Cryptography.ECC;

namespace Obsidian.Applications.ViewModels.Chat
{
    public class ProfileViewModel : ViewModelBase
    {
        readonly IFileService _fileService;
        readonly IChatEncryptionService _chatEncryptionService;
        readonly ChatWorker _chatWorker;
        readonly IVisualCrypt2Service _visualCrypt2Service;
        readonly AppRepository _repo;
        readonly IPhotoImportService _photoImportService;


        public ProfileViewModel(Container container) : base(container)
        {
            _fileService = container.Get<IFileService>();
            _chatEncryptionService = container.Get<IChatEncryptionService>();
            _chatWorker = container.Get<ChatWorker>();
            _visualCrypt2Service = container.Get<IVisualCrypt2Service>();
            _repo = container.Get<AppRepository>();
            _photoImportService = container.Get<IPhotoImportService>();

        }

        public Profile Profile
        {
            get { return Get<Profile>(); }
            private set
            {
                SetChanged(value);
            }
        }

        public object ProfileImage
        {
            get { return Get<object>(); }
            set
            {
                SetChanged(value);
            }
        }

        public override async Task OnViewDidLoad()
        {
            await LoadOrCreateProfile();
        }

        async Task<Profile> CreateAndSaveNewProfile()
        {
            ECKeyPair identityKeypair = _chatEncryptionService.GenerateIdentityKeypair().Result;

            var profile = new Profile
            {
                Id = _chatEncryptionService.GenerateId().Result,
                Name = "Me",
                IsIdentityPublished = false,
                PrivateKey = identityKeypair.PrivateKey,
                PublicKey = identityKeypair.PublicKey,
                EncryptedProfileImage = null
            };
            await _repo.AddProfile(profile);
            return profile;
        }

        public async Task LoadOrCreateProfile()
        {
            Debug.WriteLine("CALLED LOADORCREATEPROFILE");
            var setPassworResponse = _chatEncryptionService.SetMasterPassword("password");
            if (!setPassworResponse.IsSuccess)
                ; // do sth!

            IReadOnlyList<Profile> profiles = await _repo.GetProfiles();
            Profile = profiles.SingleOrDefault() ?? await CreateAndSaveNewProfile();

            Profile.IsIdentityPublished = false; // the server doesn't persit identities yet
            await _repo.UpdateProfile(Profile);

            if (Profile.EncryptedProfileImage == null)
                ProfileImage =
                    await
                        _fileService.LoadLocalImageAsync(Path.Combine(_fileService.GetInstallLocation(), "Assets\\App\\Profile.png"));
            else
            {
                var decryptedProfileImage = _visualCrypt2Service.DefaultDecrypt(Profile.EncryptedProfileImage);
                ProfileImage = await _photoImportService.ConvertPhotoBytesToPlatformImage(decryptedProfileImage);
            }

           
            var e2ERatchet = new E2ERatchet(Profile.Id, Profile.PrivateKey, _visualCrypt2Service, _repo.GetUserById, _repo.UpdateUser);
            _container.RegisterObject<E2ERatchet>(e2ERatchet,null, true); // replace existing, we'll be here every time we enter the chat feature
            _chatWorker.Init(Profile.Id, Profile.PrivateKey);
            _chatWorker.StartRunning();


        }


    }
}
