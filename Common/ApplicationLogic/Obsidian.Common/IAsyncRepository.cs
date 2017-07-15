using System.Collections.Generic;
using System.Threading.Tasks;

namespace Obsidian.Common
{
    public interface IAsyncRepository<T> where T : class, IId
    {
        Task Add(T item, string page=null);
        Task<T> Get(string id, string page = null);
        Task Delete(string id, string page = null);
        Task Update(T item,string page= null);
        Task<uint> Count(string page = null);
        Task<IReadOnlyList<T>> GetAll(string page = null);
        Task<IReadOnlyList<T>> GetRange(uint startIndex, uint maxCount, string page = null);
    }
}