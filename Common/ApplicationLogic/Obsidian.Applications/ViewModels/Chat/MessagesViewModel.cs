using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Obsidian.Applications.Infrastructure;
using Obsidian.Applications.Models.Chat;
using Obsidian.Applications.Models.Chat.MessageCollection.Framework;
using Obsidian.Applications.Services.Interfaces;
using Obsidian.Applications.Services.PortableImplementations;
using Obsidian.Applications.Workers;

namespace Obsidian.Applications.ViewModels.Chat
{
	public interface ICompositionView
	{
		void Show();
		void Hide();
	}
	public interface IMessagesView
	{
		void Show();
		void Hide();
	}
	public interface IMessageThreadView
	{
		Task OnThreadLoaded(IReadOnlyCollection<Message> messages);
		Task OnMessageAddedAsync(Message message);
		Task OnMessageEncrypted(Message message);
		Task OnMessageDecrypted(Message message);
		Task UpdateSendMessageStateFromBackgroundThread(Message message);
	}

	public enum ThreadInit
	{
		NotSet = 0,
		ViewModeChanged,
		ContactChanged
	}

	public class MessagesViewModel : ViewModelBase
	{
		public ICompositionView CompositionView;
		public IMessageThreadView MessageThreadView;
		public IMessagesView MessagesView;


		readonly GetMessagesController _getMessagesController;
		readonly IChatEncryptionService _encryptionService;


		const int Batch = 10;
		string _contactId;
		ItemIndexRange _currentItemIndexRange;



		public MessagesViewModel(Container container) : base(container)
		{
			_getMessagesController = container.Get<GetMessagesController>();
			_encryptionService = container.Get<IChatEncryptionService>();
			container.Get<ChatWorker>().OnIncomingMessagePersisted += ChatWorker_OnIncomingMessagePersisted;
		}




		public async Task TryInitializeThread(string contactId, ThreadInit threadInit)
		{
			if (threadInit == ThreadInit.ContactChanged)
			{
				_contactId = contactId; // the new contactId for the thread. 
				if (_contactId == null) // But can be null to unselect and/or if the contact was deleted. In that case, clear the thread.
				{
					MessagesView.Hide();
					CompositionView.Hide();
					await MessageThreadView.OnThreadLoaded(new Message[0]);
					return;
				}
			}
			else if (threadInit == ThreadInit.ViewModeChanged)
			{
				if (contactId == null) // In this case, we keep _contactId and init the thread with it,
				{
					MessagesView.Hide();
					CompositionView.Hide();
					return; // but when it is null we have nothing to do.
				}
					
			}
			else
				throw new InvalidOperationException();

			try
			{
				_currentItemIndexRange = new ItemIndexRange(0, Batch); // For range calculation on collections with normal order, see code before 10/31/16. Also mind this requires reverse reading.

				var messages = await _getMessagesController.LoadMessageRangeAsync(_currentItemIndexRange, contactId);

				_currentItemIndexRange = new ItemIndexRange(0, messages.Count);

				MessagesView.Show();
				CompositionView.Show();
				await MessageThreadView.OnThreadLoaded(messages);

				foreach (var message in messages)
				{
					await _encryptionService.DecryptCipherTextInVisibleBubble(message);
					await MessageThreadView.OnMessageDecrypted(message);
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}
		}



		async void ChatWorker_OnIncomingMessagePersisted(object sender, Message message)
		{
			if (message.SenderId != _contactId)
				return;

			try
			{
				await _container.Get<IDispatcher>().RunAsync(async () =>
				{
					await MessageThreadView.OnMessageAddedAsync(message);
				});

				_currentItemIndexRange = new ItemIndexRange(_currentItemIndexRange.FirstIndex,
					_currentItemIndexRange.Length + 1);

				await _encryptionService.DecryptCipherTextInVisibleBubble(message);

				await _container.Get<IDispatcher>().RunAsync(async () =>
				{
					await MessageThreadView.OnMessageDecrypted(message);
				});
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}
		}


		public async Task AddNewSendMessageToThreadBeforeEncryption(Message message)
		{
			await MessageThreadView.OnMessageAddedAsync(message);
			_currentItemIndexRange = new ItemIndexRange(_currentItemIndexRange.FirstIndex, _currentItemIndexRange.Length + 1);
		}

		static bool CanUpdateSendMessageStateInBubble(Message message, string currentContactId)
		{
			return currentContactId != null && message.RecipientId != null
				   && message.RecipientId == currentContactId;
		}

		public async Task UpdateMessageInThreadToEncryptedState(Message message)
		{
			if (CanUpdateSendMessageStateInBubble(message, _contactId))
				await MessageThreadView.OnMessageEncrypted(message);
		}

		public async Task UpdateSendMessageState(Message message)
		{
			if (CanUpdateSendMessageStateInBubble(message, _contactId))
				await MessageThreadView.UpdateSendMessageStateFromBackgroundThread(message);
		}

		public async Task<IReadOnlyCollection<Message>> OnViewMoreMessagesRequestedAsync()
		{
			var additionalRange = new ItemIndexRange(_currentItemIndexRange.FirstIndex + _currentItemIndexRange.Length, Batch);

			var additionalMessages = await _getMessagesController.LoadMessageRangeAsync(additionalRange, _contactId);
			if (additionalMessages == null)
				return await OnViewMoreMessagesRequestedAsync();
			// make the sum of current and ACTUALLY received messages
			_currentItemIndexRange = new ItemIndexRange(0, _currentItemIndexRange.Length + additionalMessages.Count);

			Debug.WriteLine($"{additionalMessages.Count} more Messages Loaded");
			// DECRYPTION
			foreach (var message in additionalMessages)
			{
				await _encryptionService.DecryptCipherTextInVisibleBubble(message);
				await MessageThreadView.OnMessageDecrypted(message);
			}
			return additionalMessages;
		}


	}
}
