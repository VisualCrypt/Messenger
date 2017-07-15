using Windows.ApplicationModel.DataTransfer;
using Obsidian.Applications.Services.Interfaces;

namespace Obsidian.UWP.Core.Services
{
    public class ClipBoardService : IClipBoardService
    {
        public void CopyText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            DataPackage dataPackage = new DataPackage {  RequestedOperation = DataPackageOperation.Copy};
            dataPackage.SetText(text);
            Clipboard.SetContent(dataPackage);
        }

        // see https://msdn.microsoft.com/en-us/library/windows/apps/mt243291.aspx
        // on how to monitor clipboard.
    }
}