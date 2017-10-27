using System.Collections.Generic;
using ObsidianMobile.Core.Interfaces.Models;
using ObsidianMobile.Core.Utils;

namespace ObsidianMobile.Core.Models
{
    public class Chat : IChat
    {
        public int Id { get; set; }
        public IList<IContact> Contacts { get; set; }

        public Chat()
        {
            Id = RandomGenerator.GenerateId();
        }
    }
}