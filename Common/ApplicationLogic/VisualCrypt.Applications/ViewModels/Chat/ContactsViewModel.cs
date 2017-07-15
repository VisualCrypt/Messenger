using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Prism.Commands;
using VisualCrypt.Applications.Data;
using VisualCrypt.Applications.Infrastructure;
using VisualCrypt.Applications.Models.Chat;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.Workers;
using VisualCrypt.Common;

namespace VisualCrypt.Applications.ViewModels.Chat
{
    public class ContactsViewModel : ViewModelBase
    {
      
        public ObservableCollection<Identity> Contacts { get; } = new ObservableCollection<Identity>();
        public ObservableCollection<Identity> SelectedContacts { get; } = new ObservableCollection<Identity>();

        readonly AppRepository _repo;

        readonly IMessageBoxService _messageBoxService;
        readonly ChatWorker _chatWorker;
        readonly MessagesViewModel _messagesViewModel;

        public ContactsViewModel(Container container) : base(container)
        {
            _repo = container.Get<AppRepository>();
            _messagesViewModel = container.Get<MessagesViewModel>();
            _messageBoxService = container.Get<IMessageBoxService>();
            _chatWorker = container.Get<ChatWorker>();
            _chatWorker.ContactUpdateReceived += async (sender, args) => await GetAllContacts();

        }

        public override async Task OnViewDidLoad()
        {
            await GetAllContacts();
        }

        public async Task GetAllContacts()
        {
            await _container.Get<IDispatcher>().RunAsync(async () =>
            {
                var allContacts = await _repo.GetAllContacts();
                Contacts.Clear();
                Contacts.AddRange(allContacts);
            });
        }

        public Identity CurrentContact
        {
            get { return Get<Identity>(); }
            set
            {
                SetChanged(value);
                InitializeMessageThread(value?.Id);
            }
        }

        async void InitializeMessageThread(string contactId)
        {
            try
            {
                await _messagesViewModel.TryInitializeThread(contactId, ThreadInit.ContactChanged);
            }
            catch (Exception e)
            {
                await _messageBoxService.ShowError(e);
            }

        }

        #region Adding a Contact

        public DelegateCommand SaveAddedContactCommand
        {
            get { return CreateCommand(ExecuteSaveAddedContactCommand, CanExecuteSaveAddedContactCommand); }
        }

        bool CanExecuteSaveAddedContactCommand()
        {
            if (string.IsNullOrEmpty(AddedContactID))
            {
                CurrentError = "VisualCrypt ID:";
                return false;
            }
            if (AddedContactID.Contains("@"))
            {
                CurrentError = "Don't type the '@'";
                return false;
            }
            if (AddedContactID.Contains(" "))
            {
                CurrentError = "Don't type Spaces!";
                return false;
            }
            if (AddedContactID.Length < 10)
            {
                CurrentError = "VisualCrypt ID: too short!";
                return false;
            }
            if (AddedContactID.Length > 10)
            {
                CurrentError = "VisualCrypt ID: too long!";
                return false;
            }

            // TODO: Verify Base64 Chars
            if (Contacts.Count(c => c.Id == AddedContactID) > 0)
            {
                CurrentError = "This Contact already exists!";
                return false;
            }
            CurrentError = "Looks good!";
            return true;

        }

        async void ExecuteSaveAddedContactCommand()
        {
            try
            {
                var addedContactId = AddedContactID;
                await AddContact(addedContactId);
                await GetAllContacts();  // while awaiting, AddedContactID will be cleared by the UI
                var added = Contacts.Single(c => c.Id == addedContactId);
                CurrentContact = added; // add this point, the contact is not yet usable, we have only tried to send the request for public key and verification to the server but we cannot have a reply yet.
            }
            catch (Exception e)
            {
                await _messageBoxService.ShowError(e.Message);
            }

        }
        async Task AddContact(string addedContactId)
        {
            await _repo.AddContact(new Identity
            {
                Id = addedContactId,
                Name = "Anonymous",
                ContactState = ContactState.Added
            });
            await _chatWorker.VerifyAddedContact(addedContactId);
        }


        public string AddedContactID
        {
            get { return Get<string>(); }
            set { SetChanged(value); }
        }
        public string CurrentError
        {
            get { return Get<string>(); }
            private set { SetChanged(value, true); }
        }

        public string ControlTitle
        {
            get { return Get<string>(); }
            private set { SetChanged(value, true); }
        }

        #endregion

        #region Delete a Contact

        public DelegateCommand DeleteContactsCommand
        {
            get { return CreateCommand(ExecuteDeleteCommand, () => true/*SelectedContacts.Count >0*/); }
        }

        public string DeleteList { get { return Get<string>(); } set { SetChanged(value); } }

        async void ExecuteDeleteCommand()
        {
            try
            {
                await _repo.DeleteContacts(SelectedContacts);
                
                Contacts.Clear();
                Contacts.AddRange(await _repo.GetAllContacts());
                CurrentContact = Contacts.FirstOrDefault(); // it's important to write to the setter again, because it triggers a message thread update. Null is allowed.
            }
            catch (Exception e)
            {
               await  _messageBoxService.ShowError(e.Message);
            }

        }




        #endregion
    }
}
