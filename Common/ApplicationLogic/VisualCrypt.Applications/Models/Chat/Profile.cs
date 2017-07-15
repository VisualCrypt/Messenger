using VisualCrypt.Applications.Infrastructure;
using VisualCrypt.Common;

namespace VisualCrypt.Applications.Models.Chat
{
    public class Profile : NotifyPropertyChangedBase, IId
    {

        #region IId

        public string Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }
        string _id;
      

        #endregion

        public byte[] EncryptedProfileImage
        {
            get { return _encryptedProfileImage; }
            set
            {
                if (_encryptedProfileImage != value)
                {
                    _encryptedProfileImage = value;
                    OnPropertyChanged();
                }
            }
        }
        byte[] _encryptedProfileImage;

        public string Name
        {
            get { return _name; }
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

        public byte[] PublicKey { get;  set; }

        public byte[] PrivateKey { get;  set; }
        public bool IsIdentityPublished { get;  set; }
    }
}
