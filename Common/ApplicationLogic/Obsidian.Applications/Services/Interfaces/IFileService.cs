
using System.Threading.Tasks;

namespace Obsidian.Applications.Services.Interfaces
{
    public interface IFileService
    {
      
        string GetLocalFolderPath();
        string GetInstallLocation();
        Task<object> LoadLocalImageAsync(string imagePath);
	    Task<object> LoadLocalImageBrushAsync(string imagePath);

		void SetLocalFolderPathForTests(string localFolderPathOverride);
    }
}
