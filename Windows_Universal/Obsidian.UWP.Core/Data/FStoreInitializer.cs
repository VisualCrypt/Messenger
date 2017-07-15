using System.Threading.Tasks;
using Obsidian.Applications.Models.Chat;
using Obsidian.UWP.Core.Storage;

namespace Obsidian.UWP.Core.Data
{
    public static class FStoreInitializer
    {
        public const string DefaultStore = "FStore";
        public static async Task InitFStore()
        {
            var fStore = new FStore(DefaultStore);
            if (!await fStore.StoreExists())
                await fStore.CreateStore();

            var profilesTable = new FSTable(nameof(Profile), IdMode.UserGenerated); // Single item, Id does not matter but speeds things up
            if (!await fStore.TableExists(profilesTable, null))
                await fStore.CreateTable(profilesTable);
            FStore.TableConfig[typeof(Profile)] = profilesTable;

            var contactsTable = new FSTable(nameof(Identity), IdMode.UserGenerated); // Id is necessary to retrieve an item
            if (!await fStore.TableExists(contactsTable, null))
                await fStore.CreateTable(contactsTable);
            FStore.TableConfig[typeof(Identity)] = contactsTable;

            var messagesTable = new FSTable(nameof(Message), IdMode.Auto, true,true); // e.g. /tbl_Message/1234567890/999999
            if (!await fStore.TableExists(messagesTable, null))                 //       /[page: recipientId]/[auto-id]
                await fStore.CreateTable(messagesTable);
            FStore.TableConfig[typeof(Message)] = messagesTable;
        }
    }
}
