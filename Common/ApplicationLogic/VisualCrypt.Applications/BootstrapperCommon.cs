using Prism.Events;
using VisualCrypt.Applications.Data;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.Services.PortableImplementations;
using VisualCrypt.Applications.ViewModels.Chat;
using VisualCrypt.Applications.Workers;
using VisualCrypt.Language.Strings;
using VisualCrypt.Common;

namespace VisualCrypt.Applications
{
    public static class BootstrapperCommon
    {
        public static void RegisterPortableTypes(Container container)
        {
            container.RegisterType<AppState, AppState>();
            container.RegisterType<ResourceWrapper, ResourceWrapper>();
        }

        public static void RegisterPortableObjects(Container container)
        {
            container.RegisterObject<AppRepository>(new AppRepository(container));
            container.RegisterObject<GetMessagesController>(new GetMessagesController(container));
            container.RegisterObject<IChatEncryptionService>(new PortableChatEncryptionService(container));

            container.RegisterObject<IChatClient>(new ChatClient(container));
            container.RegisterObject<INetworkClient>(new TLSClient(container));
            container.RegisterObject<ChatWorker>(new ChatWorker(container));
            container.RegisterObject<ProfileViewModel>(new ProfileViewModel(container));

            container.RegisterObject<MessagesViewModel>(new MessagesViewModel(container));
            container.RegisterObject<ContactsViewModel>(new ContactsViewModel(container));
          
            container.RegisterObject<SendMessagesController>(new SendMessagesController(container));
        }
    }
}
