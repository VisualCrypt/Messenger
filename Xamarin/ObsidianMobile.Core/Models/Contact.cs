using ObsidianMobile.Core.Interfaces.Models;
using ObsidianMobile.Core.Utils;

namespace ObsidianMobile.Core.Models
{
    public class Contact : IContact
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Contact(string name)
        {
            Name = name;
            Id = RandomGenerator.GenerateId();
        }

        public Contact()
        {
            Id = RandomGenerator.GenerateId();
        }
    }
}
