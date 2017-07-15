using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Obsidian.UWP.ValueConverters
{
    public class ObjectBitmapImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value;
            //return new CipherImage(new Uri("ms-appx://Assets/App/Anonymous.png"));
            var s = value as string;
            if (s != null)
            {
                try
                {
                    //return LoadImageFromPath(s);
                }
                catch { }
            }
            var bmi = new BitmapImage(new Uri("ms-appx://Assets/App/Profile.png"));

            return bmi;

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        //TaskCompletionSource<CipherImage> _tcs;
        //CipherImage LoadImageFromPath(string path)
        //{
        //    _tcs = new TaskCompletionSource<CipherImage>();
        //    var bmi = new CipherImage();
        //    bitmapImage = new CipherImage();
        //    await bitmapImage.SetSourceAsync(storageItemThumbnail);
        //    bmi.ImageOpened += (s, e) => _tcs.SetResult((CipherImage)s);


        //    _tcs.Task = Task.Run(() => { });
        //    var task Task.Run(async () =>
        //    {
        //        var imagePath = path;
        //        var storageFile = await StorageFile.GetFileFromPathAsync(imagePath);
        //        var storageItemThumbnail = await storageFile.GetThumbnailAsync(ThumbnailMode.SingleItem);
        //        CipherImage bitmapImage = null;
        //        await Container.Get<IDispatcher>().RunAsync(async () =>
        //        {
        //            bitmapImage = new CipherImage();
        //            await bitmapImage.SetSourceAsync(storageItemThumbnail);
                    
        //        });
        //        _tcs.SetResult(bitmapImage);

        //    });
        //    _tcs.Task.Wait();

        //    return _tcs.Task.Result;
        //}
    }
}

