using System.Runtime.Serialization;
using Obsidian.Applications.Infrastructure;

namespace Obsidian.Applications.Models.Settings
{
    [DataContract]
    public class ChatSettings : ViewModelBase
    {
        [DataMember]
        public string RemoteDnsHostAddress
        {
            get { return Get<string>(); }
            set { SetChanged(value); }
        }
     

        [DataMember]
        public int RemoteUdpPort
        {
            get { return Get<int>(); }
            set { SetChanged(value); }
        }

        [DataMember]
        public int RemoteTcpPort
        {
            get { return Get<int>(); }
            set { SetChanged(value); }
        }

        [DataMember]
        public int Interval
        {
            get { return Get<int>(); }
            set { SetChanged(value); }
        }

       
    }
}
