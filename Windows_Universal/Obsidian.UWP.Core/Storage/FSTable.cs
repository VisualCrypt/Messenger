using System;

namespace Obsidian.UWP.Core.Storage
{
    public class FSTable
    {


        public FSTable(string name, IdMode idMode, bool isPaged = false, bool readListInReverseOrder = false)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException();
            Name = name;
            IdMode = idMode;
            IsPaged = isPaged;
            ReadListInReverseOrder = readListInReverseOrder;
        }

        public string Name { get; }
        public IdMode IdMode { get; }
        public bool IsPaged { get; }
        public bool ReadListInReverseOrder { get; }
    }

    public enum IdMode
    {
        UserGenerated = 0,
        Auto = 1
    }

    public class FSFile
    {
        public FSFile(byte[] contents, string id = null)
        {
            if (contents == null)
                throw new ArgumentNullException();
            Id = id;
            Contents = contents;
        }

        public string Id { get; }

        public byte[] Contents { get; }
    }
}
