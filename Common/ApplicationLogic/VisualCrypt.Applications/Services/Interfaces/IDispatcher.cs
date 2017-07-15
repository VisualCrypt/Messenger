using System;
using System.Threading.Tasks;

namespace VisualCrypt.Applications.Services.Interfaces
{
    public interface IDispatcher
    {
        Task RunAsync(Action action);
    }
}
