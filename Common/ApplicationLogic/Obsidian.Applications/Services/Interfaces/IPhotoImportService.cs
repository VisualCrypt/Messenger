using System.Threading.Tasks;
using Obsidian.Applications.Models.Chat;

namespace Obsidian.Applications.Services.Interfaces
{
    public interface IPhotoImportService
    {
        Task<ImportedPhoto> ImportPhoto(string pathAndFilename);
        Task<string> GetPhotoFutureAccessPath();
        Task<byte[]> GetProfilePhotoBytes();
        Task<byte[]> GetProfilePhotoBytesFromThumbnailImage();
        Task<object> ConvertPhotoBytesToPlatformImage(byte[] photoBytes);
	    Task<object> ConvertPhotoBytesToPlatformImageBrush(byte[] photoBytes);

    }
}
