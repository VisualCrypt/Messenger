using System.Runtime.Serialization;

namespace VisualCrypt.Applications.Models.Settings
{
    [DataContract]
    public class VisualCryptSettings
    {
        [DataMember]
        public EditorSettings EditorSettings { get; set; }

        [DataMember]
        public CryptographySettings CryptographySettings { get; set; }

        [DataMember]
        public UpdateSettings UpdateSettings { get; set; }
        [DataMember]
        public ChatSettings ChatSettings { get; set; }
    }
}
