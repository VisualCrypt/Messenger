using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Obsidian.Applications.Models.Chat;
using Obsidian.Applications.Services.Interfaces;

namespace Obsidian.UWP.Core.Services
{
    public class PhotoImportService : IPhotoImportService
    {
        Dictionary<string, string> AccessTokens = new Dictionary<string, string>();

        public async Task<object> ConvertPhotoBytesToPlatformImage(byte[] photoBytes)
        {
            try
            {
                InMemoryRandomAccessStream ras2 = await ConvertTo(photoBytes);
                var bmi = new BitmapImage();
                await bmi.SetSourceAsync(ras2);
				return bmi;
            }
            catch (Exception)
            {
                return null;
            }
        
            //using (var ms = new MemoryStream(photoBytes))
            //{
            //    var bmi = new BitmapImage();
            //    bmi.DecodePixelHeight = 100;
            //    bmi.DecodePixelWidth = 100;
            //    var ras = ms.AsRandomAccessStream();
            //    ras.Seek(0);
            //    if(ras.CanRead)
            //        await bmi.SetSourceAsync(ras);
            //    return bmi;
            //}

        }

	    public async Task<object> ConvertPhotoBytesToPlatformImageBrush(byte[] photoBytes)
	    {
		    try
		    {
			    InMemoryRandomAccessStream ras2 = await ConvertTo(photoBytes);
			    var bmi = new BitmapImage();
			    await bmi.SetSourceAsync(ras2);
				var imageBrush = new ImageBrush();
			    imageBrush.ImageSource = bmi;
				imageBrush.AlignmentX = AlignmentX.Center;
			    imageBrush.AlignmentY = AlignmentY.Center;
				imageBrush.Stretch = Stretch.UniformToFill;
				return imageBrush;
		    }
		    catch (Exception)
		    {
			    return null;
		    }

		    //using (var ms = new MemoryStream(photoBytes))
		    //{
		    //    var bmi = new BitmapImage();
		    //    bmi.DecodePixelHeight = 100;
		    //    bmi.DecodePixelWidth = 100;
		    //    var ras = ms.AsRandomAccessStream();
		    //    ras.Seek(0);
		    //    if(ras.CanRead)
		    //        await bmi.SetSourceAsync(ras);
		    //    return bmi;
		    //}

	    }

		public async Task<string> GetPhotoFutureAccessPath()
        {
            FileOpenPicker picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.PicturesLibrary };
            picker.FileTypeFilter.Add("*");
            StorageFile fileToOpen = await picker.PickSingleFileAsync();
            if (fileToOpen == null)
                return null;

            AccessTokens[fileToOpen.Path] = StorageApplicationPermissions.FutureAccessList.Add(fileToOpen, fileToOpen.Path);
            return fileToOpen.Path;
        }

        public async Task<byte[]> GetProfilePhotoBytesFromThumbnailImage()
        {
            FileOpenPicker picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.PicturesLibrary };
            picker.FileTypeFilter.Add("*");
            StorageFile fileToOpen = await picker.PickSingleFileAsync();
            if (fileToOpen == null)
                return null;
            StorageItemThumbnail thumb = await fileToOpen.GetThumbnailAsync(ThumbnailMode.SingleItem);
            using (var stream = thumb.AsStreamForRead((int)thumb.Size))
            {
                byte[] bytes = new byte[stream.Length];
                await stream.ReadAsync(bytes, 0, bytes.Length);
                return bytes;
            }
        }

        public async Task<byte[]> GetProfilePhotoBytes()
        {
            FileOpenPicker picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.PicturesLibrary };
            picker.FileTypeFilter.Add("*");
            StorageFile fileToOpen = await picker.PickSingleFileAsync();
            if (fileToOpen == null)
                return null;

            var read = await FileIO.ReadBufferAsync(fileToOpen);
            return read.ToArray();
        }

        // Resize: https://dreamteam-mobile.com/blog/2016/05/uwp-resize-convert-change-jpeg-quality-image-manipulation/
        public async  Task<ImportedPhoto> ImportPhoto(string pathAndFilename)
        {
            IStorageFile storagefile;
            var token = GetTokenByPathAndFilename(pathAndFilename);
            if (token != null)
            {
                storagefile = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(token);
            }
            else
            {
                storagefile = await StorageFile.GetFileFromPathAsync(pathAndFilename);
            }
            var photo = new ImportedPhoto();
          
            var buffer  = await FileIO.ReadBufferAsync(storagefile);
            photo.OriginalFileBytes = buffer.ToArray(0, (int)buffer.Length);
            return photo;
        }
        static async Task<InMemoryRandomAccessStream> ConvertTo(byte[] arr)
        {
            InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
            await randomAccessStream.WriteAsync(arr.AsBuffer());
            randomAccessStream.Seek(0); // Just to be sure.
                                        // I don't think you need to flush here, but if it doesn't work, give it a try.
            return randomAccessStream;
        }

        string GetTokenByPathAndFilename(string pathAndFilename)
        {
            if (AccessTokens.ContainsKey(pathAndFilename))
                return AccessTokens[pathAndFilename];
            return null;
        }
    }
}
