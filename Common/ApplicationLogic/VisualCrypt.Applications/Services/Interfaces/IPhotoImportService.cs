using System.Threading.Tasks;
using VisualCrypt.Applications.Models.Chat;

namespace VisualCrypt.Applications.Services.Interfaces
{
    public interface IPhotoImportService
    {
        Task<ImportedPhoto> ImportPhoto(string pathAndFilename);
        Task<string> GetPhotoFutureAccessPath();
        Task<byte[]> GetProfilePhotoBytes();
        Task<byte[]> GetProfilePhotoBytesFromThumbnailImage();
        Task<object> ConvertPhotoBytesToPlatformImage(byte[] photoBytes);
    }
}
