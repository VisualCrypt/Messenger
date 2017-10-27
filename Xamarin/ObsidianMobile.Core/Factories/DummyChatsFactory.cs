using System.Collections.Generic;
using System.Collections.ObjectModel;
using ObsidianMobile.Core.Interfaces.Models;
using ObsidianMobile.Core.Models;

namespace ObsidianMobile.Core.Factories
{
    public class DummyChatsFactory
    {
        public static ObservableCollection<IChat> GetChats()
        {
            return new ObservableCollection<IChat>
            {
                new Chat {Id = 208145205, Contacts = new List<IContact> {Server.CurrentUser, new Contact { Name = "Peter McClory", Id = 1513687254 }}},
                new Chat {Id = 699981449, Contacts = new List<IContact> {Server.CurrentUser, new Contact { Name = "Lisa McClory", Id = 578309888 }}},
                new Chat {Id = 1270834840, Contacts = new List<IContact> {Server.CurrentUser, new Contact { Name = "Claus Ehrenberg", Id = 1086367793 }}},
                new Chat {Id = 561577645, Contacts = new List<IContact> {Server.CurrentUser, new Contact { Name = "Michael Wilson", Id = 1787772043 }}},
            };
        }
    }
}
