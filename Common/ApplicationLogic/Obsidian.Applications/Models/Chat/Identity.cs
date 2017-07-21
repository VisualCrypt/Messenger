using System;
using Obsidian.Applications.Infrastructure;
using Obsidian.Common;

namespace Obsidian.Applications.Models.Chat
{
    public class Identity : NotifyPropertyChangedBase, IId
    {
        #region IId

        public string Id { get; set; }

        #endregion

	    public string Name
	    {
		    get => _name;
		    set
		    {
			    if (_name != value)
			    {
				    _name = value;
					OnPropertyChanged();
			    }
		    }
	    }

	    string _name;

        public byte[] Image { get; set; }
        public DateTime LastSeenUtc { get; set; }
        public DateTime FirstSeenUtc { get; set; }
        public byte[] StaticPublicKey { get; set; }
        public ContactState ContactState { get; set; }
        public byte[] CryptographicInformation { get; set; }

	    public object ContactImageBrush
	    {
		    get => _contactImageBrush;
		    set
		    {
			    if (_contactImageBrush != value)
			    {
				    _contactImageBrush = value;
				    OnPropertyChanged();
			    }
		    }
	    }
	    object _contactImageBrush;

	}
}

