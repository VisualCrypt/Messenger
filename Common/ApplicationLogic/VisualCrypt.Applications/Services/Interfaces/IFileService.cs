using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using VisualCrypt.Applications.Models;
using VisualCrypt.Cryptography.VisualCrypt2.Infrastructure;

namespace VisualCrypt.Applications.Services.Interfaces
{
    public interface IFileService
    {
      
        string GetLocalFolderPath();
        string GetInstallLocation();
        Task<object> LoadLocalImageAsync(string imagePath);
        void SetLocalFolderPathForTests(string localFolderPathOverride);
    }
}
