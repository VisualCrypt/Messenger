using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Obsidian.Common;
using Obsidian.Cryptography.Api.Implementations;
using Obsidian.Cryptography.Api.Infrastructure;
using Obsidian.MessageNode.Core.Server;
using Obsidian.MessageNode.SqlServer.Models;

namespace Obsidian.MessageNode.Core.Data
{
	class SqlRepository : IServerRepository
	{
		readonly InfoContext _context = new InfoContext();
		readonly DateTime _now = DateTime.UtcNow;

		public async Task AddIdentity(XIdentity identity)
		{
			if (!CheckNewIdentity(identity))
				return;

			UserInfo existing = await _context.UserInfos.FindAsync(identity.ID);
			if (existing == null)
			{
				var userInfo = new UserInfo
				{
					Id = identity.ID,
					PublicKey = new string(Base64Encoder.EncodeDataToBase64CharArray(identity.PublicIdentityKey)),
					FirstSeenUTC = _now,
					LastSeenUTC = _now
				};
				_context.UserInfos.Add(userInfo);
				await _context.SaveChangesAsync();
				return;
			}
			if (!ByteArrays.AreAllBytesEqual(identity.PublicIdentityKey,
				Base64Encoder.DecodeBase64StringToBinary(existing.PublicKey)))
			{
				Error($"Different new PublicKey for {identity.ID}. Ignoring request!");
				return;
			}
			existing.LastSeenUTC = _now;

			await _context.SaveChangesAsync();
		}

		bool CheckNewIdentity(XIdentity identity)
		{
			if (identity == null)
				return Error("Identity null");
			if (identity.ID == null)
				return Error("Identity.ID null");
			if (identity.PublicIdentityKey == null)
				return Error("Identity.PublicIdentityKey null");
			if (identity.PublicIdentityKey.Length != 32)
				return Error("Identity.PublicIdentityKey.Lenght != 32");
			if (ByteArrays.AreAllBytesZero(identity.PublicIdentityKey))
				return Error("Identity.PublicIdentityKey - All Bytes zero!");
			return true;
		}

		public async Task<XIdentity> GetIdentityAsync(string identityId)
		{
			var requestedIdentity = new XIdentity { ID = identityId };

			var foundIdentity = await _context.UserInfos.FindAsync(identityId);
			if (foundIdentity == null)
			{
				requestedIdentity.ContactState = ContactState.NonExistent;
			}
			//else if (foundIdentity.ContactState == ContactState.Revoked)
			//{
			//	requestedIdentity.ContactState = ContactState.Revoked;
			//}
			else
			{
				//if (foundIdentity.ContactState != ContactState.Valid)
				//	throw new Exception("Expected a state of 'Publihed'");
				requestedIdentity.ContactState = ContactState.Valid;
				requestedIdentity.FirstSeenUTC = foundIdentity.FirstSeenUTC;
				requestedIdentity.LastSeenUTC = foundIdentity.LastSeenUTC;
				requestedIdentity.PublicIdentityKey = Base64Encoder.DecodeBase64StringToBinary(foundIdentity.PublicKey);
			}
			return requestedIdentity;
		}

		public async Task<string> AddMessage(XMessage message)
		{
			var m = new Message
			{
				Id = Guid.NewGuid(),
				XMessageId = message.Id,
				SenderId = message.SenderId,
				RecipientId = message.RecipientId,
				MessageType = (int) message.MessageType,
				EncryptedDateUtc = message.EncryptedDateUtc,
				TextCipher = message.TextCipher,
				ImageCipher = message.ImageCipher,
				DynamicPublicKey = message.DynamicPublicKey,
				DynamicPublicKeyId = message.DynamicPublicKeyId,
				PrivateKeyHint = message.PrivateKeyHint

			};
			_context.Messages.Add(m);
			await _context.SaveChangesAsync();
			
			return $"{message.Id};{message.RecipientId}";
		}

		public async Task<bool> AnyNews(string myId)
		{
			var count = await _context.Messages.CountAsync(m => m.RecipientId == myId);
			return count > 0;
		}

		

		public async  Task<List<XMessage>> GetMessages(string myId)
		{
			// TODO: remove only after successful download
			var messages = await _context.Messages.Where(m => m.RecipientId == myId).Take(3).ToListAsync();
			var xmessages = new List<XMessage>();
			foreach (var msg in messages)
			{
				var m = new XMessage
				{
					Id = msg.XMessageId,
					SenderId = msg.SenderId,
					RecipientId = msg.RecipientId,
					MessageType = (MessageType) msg.MessageType,
					EncryptedDateUtc = msg.EncryptedDateUtc,
					TextCipher = msg.TextCipher,
					ImageCipher = msg.ImageCipher,
					DynamicPublicKey = msg.DynamicPublicKey,
					DynamicPublicKeyId = msg.DynamicPublicKeyId,
					PrivateKeyHint = msg.PrivateKeyHint
				};
				xmessages.Add(m);
			}
			_context.Messages.RemoveRange(messages);
			await _context.SaveChangesAsync();
			return xmessages;
		}

		bool Error(string message, [CallerMemberName] string method = null)
		{
			Log.Error($"{DateTime.UtcNow} SqlRepository {method}: {message}");
			return false;
		}
	}
}
