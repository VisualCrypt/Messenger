using System.Collections.ObjectModel;
using System.Diagnostics;
using ObsidianMobile.Core.Interfaces.Models;
using ObsidianMobile.Core.Models;
using ObsidianMobile.Core.Utils;

namespace ObsidianMobile.Core.Factories
{
    public static class DummyContactsFactory
    {
        public static ObservableCollection<IContact> GetContacts()
        {
            return new ObservableCollection<IContact>
            {
                new Contact() {Name = "Peter McClory", Id = 1513687254},
                new Contact() {Name = "Lisa McClory", Id = 578309888},
                new Contact() {Name = "Claus Ehrenberg", Id = 1086367793},
                new Contact() {Name = "Michael Wilson", Id = 1787772043},
            };
        }
    }
}