using System.Threading.Tasks;

namespace Obsidian.Common
{
    public interface IRequestHandler
    {
        Task<byte[]> ProcessRequestAsync(byte[] rawRequest);
    }
}