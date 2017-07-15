using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using VisualCrypt.UWP.Core;

namespace VisualCrypt.UWP.TestApp
{
    [TestClass]
    public class FStoreTests
    {
        readonly FStore _fStore;

        public FStoreTests()
        {
            _fStore = new FStore("myStoragePath");
            Debug.WriteLine(ApplicationData.Current.LocalFolder.Path);
        }

        [TestInitialize]
        public async Task TestInit()
        {
            await _fStore.DeleteStoreIfExists();
        }

        [TestMethod]
        public async Task Create_Check_Delete_Store()
        {
            if (await _fStore.StoreExists())
                Assert.Fail();
            await _fStore.CreateStore();
            if (!await _fStore.StoreExists())
                Assert.Fail();
            await _fStore.DeleteStoreIfExists();
            if (await _fStore.StoreExists())
                Assert.Fail();

        }

        [TestMethod]
        public async Task Create_Store_Throws_If_Already_Exists()
        {
            if (await _fStore.StoreExists())
                Assert.Fail();
            await _fStore.CreateStore();
            bool didThrow = false;
            try
            {
                await _fStore.CreateStore();
            }
            catch (Exception)
            {
                didThrow = true;
            }
            Assert.IsTrue(didThrow);
        }

        [TestMethod]
        public async Task Create_Store_AddItems_Delete_Store()
        {

            await _fStore.CreateStore();
            var table = new FSTable("myTable", IdMode.Auto);

            await _fStore.CreateTable(table);

            var dataItem = new byte[1024 * 1024];
            var file = new FSFile(dataItem);

            for (var i = 0; i < 100; i++)
            {
                await _fStore.InsertFile(table, null, null, null, null); // TODO Repair Test
            }

            var store2 = new FStore("myStoragePath");
            for (var i = 0; i < 100; i++)
            {
                await _fStore.InsertFile(table, null, null, null, null); // TODO Repair Test
            }

            await _fStore.DeleteStoreIfExists();
            if (await _fStore.StoreExists())
                Assert.Fail();
            if (await store2.StoreExists())
                Assert.Fail();
        }

        [TestMethod]
        public async Task Get_File()
        {
            await _fStore.CreateStore();
            var table = new FSTable("myTable", IdMode.Auto);

            await _fStore.CreateTable(table);

            var dataItem = new byte[1024 * 1024];
            var file = new FSFile(dataItem);
            var storedFileId = await _fStore.InsertFile(table, null, null, null, null); // TODO Repair Test

            var retrievedFile = await _fStore.FindFile(table, storedFileId,null);
            Assert.IsTrue(retrievedFile.Contents.Length == dataItem.Length);

            var wontFindMe = await _fStore.FindFile(table, "123",null);
            Assert.IsNull(wontFindMe);
        }

        [TestMethod]
        public async Task Delete_File()
        {
            await _fStore.CreateStore();
            var table = new FSTable("myTable", IdMode.Auto);

            await _fStore.CreateTable(table);

            var dataItem = new byte[1024 * 1024];
            var file = new FSFile(dataItem);
            var storedFileId = await _fStore.InsertFile(table, null, null,null,null); // TODO Repair Test

            var retrievedFile = await _fStore.FindFile(table, storedFileId,null);
            Assert.IsTrue(retrievedFile.Contents.Length == dataItem.Length);

            await _fStore.DeleteFileIfExists(table, storedFileId, null);
            var deletedFile = await _fStore.FindFile(table, storedFileId, null);
            Assert.IsNull(deletedFile);
        }

        [TestMethod]
        public async Task Update_File()
        {
            await _fStore.CreateStore();
            var table = new FSTable("myTable", IdMode.Auto);

            await _fStore.CreateTable(table);

            var dataItem = new byte[1024 * 1024];
            var file = new FSFile(dataItem);
            var storedFileId = await _fStore.InsertFile(table, null, null, null, null); // TODO Repair Test

            var updated = new FSFile(new byte[1024], storedFileId);
            await _fStore.UpdateFile(table, updated, null);

            var reloaded = await _fStore.FindFile(table, storedFileId, null);
            Assert.IsTrue(reloaded.Contents.Length == updated.Contents.Length);
        }
    }
}
