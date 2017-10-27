using System;
using System.Collections.Generic;

namespace ObsidianMobile.Core.Interfaces.Models
{
    public interface IChat
    {
        int Id { get; set; }
        IList<IContact> Contacts { get; set; }
    }
}
