using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;
using Obsidian.Applications.Services.Interfaces;

namespace Obsidian.UWP.Core.Services
{
    public class FileService : IFileService
    {
        public readonly Dictionary<string, string> AccessTokens;


        public FileService(Container container)
        {
            AccessTokens = new Dictionary<string, string>();
        }

        public string GetLocalFolderPath()
        {
            return _localFolderPathOverrride ?? ApplicationData.Current.LocalFolder.Path;
        }

        string _localFolderPathOverrride;
        public void SetLocalFolderPathForTests(string localFolderPathOverrride)
        {
            if (!Directory.Exists(localFolderPathOverrride))
                Directory.CreateDirectory(localFolderPathOverrride);
            _localFolderPathOverrride = localFolderPathOverrride;
        }


        public string GetInstallLocation()
        {
            return Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
        }

        public async Task<object> LoadLocalImageAsync(string imagePath)
        {
            var storageFile = await StorageFile.GetFileFromPathAsync(imagePath);
            var storageItemThumbnail = await storageFile.GetThumbnailAsync(ThumbnailMode.SingleItem);
            var bitmapImage = new BitmapImage();
            await bitmapImage.SetSourceAsync(storageItemThumbnail);
            return bitmapImage;
        }




      
    }
}
