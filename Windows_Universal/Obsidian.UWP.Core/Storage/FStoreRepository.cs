using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Obsidian.Applications.Models.Serialization;
using Obsidian.Common;

namespace Obsidian.UWP.Core.Storage
{
    public class FStoreRepository<T> : IAsyncRepository<T> where T : class, IId
    {
        static FSTable Table => FStore.TableConfig[typeof(T)];

        readonly FStore _fStore;

        public FStoreRepository(string storeFolderName)
        {
            _fStore = new FStore(storeFolderName);
        }

        public async Task Add(T item, string page = null)
        {
            if (Table.IdMode == IdMode.Auto && item.Id != null)
                throw new InvalidOperationException();
            if (Table.IsPaged && page == null)
                throw new InvalidOperationException();
            Func<T, byte[]> serFun = RepositorySerializer.Serialize;
            item.Id = await _fStore.InsertFile(Table, item.Id, UpdateIdAndSerialize, serFun, item, page);
        }

        byte[] UpdateIdAndSerialize(string itemId, object serFun, object entity)
        {
            Func<T, byte[]> fun = (Func<T, byte[]>)serFun;
            T actualEntity = (T)entity;
            actualEntity.Id = itemId;
            return fun(actualEntity);
        }

        public async Task<T> Get(string id, string page = null)
        {
            var file = await _fStore.FindFile(Table, id, page);
            return RepositorySerializer.Deserialize<T>(file.Contents);
        }

        public async Task Delete(string id, string page = null)
        {
            await _fStore.DeleteFileIfExists(Table, id, page);
        }

        public async Task Update(T item, string page)
        {
            var contents = RepositorySerializer.Serialize(item);
            await _fStore.UpdateFile(Table, new FSFile(contents, item.Id), page);
        }

        public async Task<uint> Count(string page = null)
        {
            return await _fStore.CountFiles(Table, page);
        }

        public async Task<IReadOnlyList<T>> GetAll(string page = null)
        {
            var files = await _fStore.GetAll(Table, page);
            var entities = new T[files.Count];
            for (var i = 0; i < files.Count; i++)
                entities[i] =  RepositorySerializer.Deserialize<T>(files[i].Contents);
            return entities;
        }

        public async Task<IReadOnlyList<T>> GetRange(uint startIndex, uint maxCount, string page = null)
        {
            var files = await _fStore.GetRange(Table, startIndex, maxCount, page);
            var entities = new T[files.Count];
            for (var i = 0; i < files.Count; i++)
                entities[i] = RepositorySerializer.Deserialize<T>(files[i].Contents);
            return entities;
        }
    }
}
