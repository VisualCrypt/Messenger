using System;
using System.Threading.Tasks;

namespace Obsidian.Applications.Services.Interfaces
{
    public interface IDispatcher
    {
        Task RunAsync(Action action);
    }
}
