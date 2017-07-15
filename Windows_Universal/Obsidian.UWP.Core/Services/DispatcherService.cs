using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Obsidian.Applications.Services.Interfaces;

namespace Obsidian.UWP.Core.Services
{
    public class CoreDispatcherService : IDispatcher
    {
        public async Task RunAsync(Action action)
        {
            if (!CoreApplication.MainView.CoreWindow.Dispatcher.HasThreadAccess)
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => action());
            else
            {
                action();
            }

        }

    }
}
