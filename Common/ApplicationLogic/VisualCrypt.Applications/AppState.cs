namespace VisualCrypt.Applications
{
    public class AppState
    {
        public bool HasValidContacts { get; private set; }
        public bool IsMessagesWaiting { get; private set; }
        public bool IsIdentityPublished { get; private set; }
        public bool IsUdpConnected { get; internal set; }

        public void SetIsMessagesWaiting(bool isMessagesWaiting)
        {
           IsMessagesWaiting = isMessagesWaiting;
        }

       

        internal void SetIsIdentityPublished(bool isIdentityPublished)
        {
            IsIdentityPublished = isIdentityPublished;
        }

        internal void SetHasValidContacts(bool hasValidContacts)
        {
            HasValidContacts = hasValidContacts;
        }

        internal void SetUdpIsConnected(bool v)
        {
            IsUdpConnected = v;
        }
    }
}
