using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Obsidian.Applications.Data;
using Obsidian.Applications.Infrastructure;
using Obsidian.Applications.Models.Chat;
using Obsidian.Applications.Mvvm.Commands;
using Obsidian.Applications.Services.Interfaces;
using Obsidian.Applications.Services.PortableImplementations;
using Obsidian.Applications.Workers;
using Obsidian.Common;
using Obsidian.Cryptography.Api.Interfaces;

namespace Obsidian.Applications.ViewModels.Chat
{
	public class ContactsViewModel : ViewModelBase
	{

		public ObservableCollection<Identity> Contacts { get; } = new ObservableCollection<Identity>();
		public ObservableCollection<Identity> SelectedContacts { get; } = new ObservableCollection<Identity>();

		readonly AppRepository _repo;

		readonly IMessageBoxService _messageBoxService;
		readonly ChatWorker _chatWorker;
		readonly MessagesViewModel _messagesViewModel;


		readonly IFileService _fileService;
		readonly IVisualCrypt2Service _visualCrypt2Service;
		readonly IPhotoImportService _photoImportService;

		public ContactsViewModel(Container container) : base(container)
		{
			_repo = container.Get<AppRepository>();
			_messagesViewModel = container.Get<MessagesViewModel>();
			_messageBoxService = container.Get<IMessageBoxService>();

			_fileService = container.Get<IFileService>();
			_visualCrypt2Service = container.Get<IVisualCrypt2Service>();
			_photoImportService = container.Get<IPhotoImportService>();

			_chatWorker = container.Get<ChatWorker>();
			_chatWorker.ContactUpdateReceived += async (sender, args) => await GetAllContactsBg();

		}

		public override async Task OnViewDidLoad()
		{
			await GetAllContacts();
		}

		async Task GetAllContactsBg()
		{
			await _container.Get<IDispatcher>().RunAsync(async () =>
			{
				await GetAllContacts();
			});
		}

		async Task GetAllContacts()
		{
			var allContacts = await _repo.GetAllContacts();
			foreach (var c in allContacts)
			{
				c.ContactImageBrush = await GetContactImageBrush(c);
			}
			Contacts.Clear();
			Contacts.AddRange(allContacts);
		}

		async Task<object> GetContactImageBrush(Identity contact)
		{
			object imageBrush = null;
			if (contact.Image == null)
				imageBrush =
					await
						_fileService.LoadLocalImageBrushAsync(Path.Combine(_fileService.GetInstallLocation(), "Assets\\App\\Profile.png"));
			else
			{
				var decryptedProfileImage = _visualCrypt2Service.DefaultDecrypt(contact.Image);
				imageBrush = await _photoImportService.ConvertPhotoBytesToPlatformImageBrush(decryptedProfileImage);
			}
			return imageBrush;
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
				CurrentError = "Obsidian ID:";
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
				CurrentError = "Obsidian ID: too short!";
				return false;
			}
			if (AddedContactID.Length > 10)
			{
				CurrentError = "Obsidian ID: too long!";
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
				await GetAllContacts();
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
				Name = "Unverified Contact",
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
			set { SetChanged(value, true); }
		}

		#endregion

		public Identity ContactToEdit
		{
			get { return Get<Identity>(); }
			set
			{
				NewName = value.Name;
				SetChanged(value, true);
				
			}
		}

		public string RenameError
		{
			get { return Get<string>(); }
			set { SetChanged(value, true); }
		}

		public string NewName
		{
			get { return Get<string>(); }
			set { SetChanged(value); }
		}

		public DelegateCommand RenameContactCommand => CreateCommand(ExecuteRenameContactCommand, CanExecuteRenameContactCommand);

		bool CanExecuteRenameContactCommand()
		{
			if (string.IsNullOrEmpty(NewName))
			{
				RenameError = "Rename: Too short!";
				return false;
			}
			
			if (NewName.Length > 50)
			{
				RenameError = "Rename: Too long!";
				return false;
			}
			if (NewName == ContactToEdit.Name)
			{
				RenameError = "Rename: No Change...";
				return false;
			}
			RenameError = "Rename:";
			return true;

		}

		async void ExecuteRenameContactCommand()
		{
			try
			{
				await _repo.UpdateContactName(ContactToEdit.Id,NewName);
				ContactToEdit.Name = NewName;
			}
			catch (Exception e)
			{
				await _messageBoxService.ShowError(e.Message);
			}
		}

		

	

		public string ControlTitle
		{
			get { return Get<string>(); }
			private set { SetChanged(value, true); }
		}

	

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

				await GetAllContacts();
				CurrentContact = Contacts.FirstOrDefault(); // it's important to write to the setter again, because it triggers a message thread update. Null is allowed.
			}
			catch (Exception e)
			{
				await _messageBoxService.ShowError(e.Message);
			}

		}




		#endregion
	}
}
