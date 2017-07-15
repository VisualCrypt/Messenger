using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace Obsidian.UWP.Core.Storage
{
    public class FStore
    {
        public const string TablePrefix = "tbl_";
        public static readonly ConcurrentDictionary<Type, FSTable> TableConfig = new ConcurrentDictionary<Type, FSTable>();
        static readonly ConcurrentDictionary<string, uint> IdCache = new ConcurrentDictionary<string, uint>();


        readonly string _storagePath;



        public FStore(string path)
        {
            _storagePath = path;
        }

        public async Task CreateStore()
        {
            await ApplicationData.Current.LocalFolder.CreateFolderAsync(_storagePath, CreationCollisionOption.FailIfExists);
        }



        public async Task CreateTable(FSTable table)
        {
            var store = await FindStoreFolder();
            await store.CreateFolderAsync(table.TableFolderName(), CreationCollisionOption.FailIfExists);
        }

        public async Task<string> InsertFile(FSTable table, string itemId, Func<string, object, object, byte[]> doSerialize, object serFun, object entity, string page = null, int currentAttmept = 1)
        {
            if (table == null || doSerialize == null)
                throw new ArgumentException();
            var tableFolder = await FindTableFolder(table, page);

            if (table.IdMode == IdMode.UserGenerated)
            {
                StorageFile newFile = await tableFolder.CreateFileAsync(itemId, CreationCollisionOption.FailIfExists);
                await FileIO.WriteBytesAsync(newFile, doSerialize(newFile.Name, serFun, entity));
                return newFile.Name;
            }

            string nextId = await GetNextId(tableFolder);
            try
            {
                StorageFile newFile = await tableFolder.CreateFileAsync(nextId, CreationCollisionOption.FailIfExists);
                await FileIO.WriteBytesAsync(newFile, doSerialize(newFile.Name, serFun, entity));
                return newFile.Name;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine($"Insert Attempt {currentAttmept} failed.");
                uint dummy;
                IdCache.TryRemove(tableFolder.Name, out dummy);
                if (currentAttmept > 10)
                    throw;
                return await InsertFile(table, itemId, doSerialize, serFun, entity, page, currentAttmept + 1);
            }
        }

        public async Task<FSFile> FindFile(FSTable table, string id, string page)
        {
            if (table == null || id == null)
                throw new ArgumentException();
            var tableFolder = await FindTableFolder(table, page);
            var storageFile = await tableFolder.TryGetItemAsync(id);
            if (storageFile == null)
                return null;
            var buffer = await FileIO.ReadBufferAsync((StorageFile)storageFile);
            return new FSFile(buffer.ToArray(), id);
        }

        public async Task DeleteFileIfExists(FSTable table, string id, string page)
        {
            if (table == null || id == null)
                throw new ArgumentException();
            var tableFolder = await FindTableFolder(table, page);
            var storageFile = await tableFolder.TryGetItemAsync(id);
            if (storageFile == null)
                return;
            await storageFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
        }

        public async Task UpdateFile(FSTable table, FSFile file, string page)
        {
            if (table == null || file?.Id == null)
                throw new ArgumentException();
            var tableFolder = await FindTableFolder(table, page);
            var storageFile = await tableFolder.GetFileAsync(file.Id);
            await FileIO.WriteBytesAsync(storageFile, file.Contents);
        }

        public async Task<IReadOnlyList<FSFile>> GetRange(FSTable table, uint startIndex, uint maxCount, string page)
        {
            var tableFolder = await FindTableFolder(table, page);
            var tableFiles = await tableFolder.GetFilesAsync(CommonFileQuery.OrderByName, startIndex, maxCount);
            return await ReadFiles(table, tableFiles);
        }

        public async Task<IReadOnlyList<FSFile>> GetAll(FSTable table, string page)
        {
            var tableFolder = await FindTableFolder(table, page);
            var tableFiles = await tableFolder.GetFilesAsync(CommonFileQuery.OrderByName);
            return await ReadFiles(table, tableFiles);
        }

        async Task<List<FSFile>> ReadFiles(FSTable table, IReadOnlyList<StorageFile> tableFiles)
        {
            var files = new List<FSFile>();
            if (!table.ReadListInReverseOrder)
                for (var i = 0; i < tableFiles.Count; i++)
                {
                    var buffer = await FileIO.ReadBufferAsync(tableFiles[i]);
                    files.Add(new FSFile(buffer.ToArray(), tableFiles[i].Name));
                }
            else
            {
                for (var i = tableFiles.Count - 1; i >= 0; i--)
                {
                    var buffer = await FileIO.ReadBufferAsync(tableFiles[i]);
                    files.Add(new FSFile(buffer.ToArray(), tableFiles[i].Name));
                }
            }
            return files;
        }
        public async Task<uint> CountFiles(FSTable table, string page)
        {
            var tableFolder = await FindTableFolder(table, page);
            var query = tableFolder.CreateFileQuery();
            uint count = await query.GetItemCountAsync();
            return count;
        }

        async Task<string> GetNextId(StorageFolder tableFolder)
        {
            uint lastIdFromCache;
            if (IdCache.TryGetValue(tableFolder.Name, out lastIdFromCache))
            {
                var newId = lastIdFromCache - 1;
                IdCache[tableFolder.Name] = newId;
                return newId.ToString("d6");
            }
            var tableFiles = await tableFolder.GetFilesAsync(CommonFileQuery.OrderByName, 0, 1);

            uint lastId;
            if (tableFiles.Count == 0)
                lastId = 999999;
            else
            {
                var lastTableFile = tableFiles[0];
                if (!uint.TryParse(lastTableFile.Name, NumberStyles.None, null, out lastId))
                {
                    // we delete everything that spoils our system!
                    await tableFiles[0].DeleteAsync(StorageDeleteOption.Default);
                    await GetNextId(tableFolder);
                }
            }
            uint nextId = lastId - 1; // we'll never return 999999, the fist id will be 999998
            IdCache[tableFolder.Name] = nextId;
            var nextFileName = nextId.ToString("d6");
            return nextFileName;
        }

        public async Task<bool> StoreExists()
        {
            var storeFolder = await FindStoreFolder();
            return storeFolder != null;
        }

        public async Task<bool> TableExists(FSTable table, string page)
        {
            var tableFolder = await FindTableFolder(table, page);
            return tableFolder != null;
        }

        public async Task DeleteStoreIfExists()
        {
            var dbFolder = await FindStoreFolder();
            if (dbFolder == null)
                return;
            await dbFolder.DeleteAsync(StorageDeleteOption.PermanentDelete);
        }

        public async Task DeleteTableIfExists(FSTable table, string page)
        {
            var tableFolder = await FindTableFolder(table, page);
            if (tableFolder == null)
                return;
            await tableFolder.DeleteAsync(StorageDeleteOption.PermanentDelete);
        }

        async Task<StorageFolder> FindStoreFolder()
        {
            var storeFolder = await ApplicationData.Current.LocalFolder.TryGetItemAsync(_storagePath);
            if (storeFolder == null)
                return null;
            if (!storeFolder.IsOfType(StorageItemTypes.Folder))
                throw new Exception();
            return (StorageFolder)storeFolder;
        }

        async Task<StorageFolder> FindTableFolder(FSTable table, string page)
        {
            StorageFolder storeFolder = await FindStoreFolder();
            StorageFolder tableFolder = (StorageFolder)await storeFolder.TryGetItemAsync(table.TableFolderName());
            if (tableFolder == null)
                return null;
            if (!table.IsPaged || page == null)
                return tableFolder;
            StorageFolder pageFolder = await tableFolder.CreateFolderAsync(page, CreationCollisionOption.OpenIfExists);
            return pageFolder;
        }

    }

    public static class FStoreExtensions
    {
        public static string TableFolderName(this FSTable table)
        {
            return FStore.TablePrefix + table.Name;
        }


    }
}
