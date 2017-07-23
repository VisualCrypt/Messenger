using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Obsidian.Common;
using Obsidian.MessageNode.SqlServer.Models;

namespace Obsidian.MessageNode.Core.Data
{
	class SqlRepository : IServerRepository
	{
		static InfoContext Context => new InfoContext();
		
		public async Task AddIdentity(XIdentity identity)
		{
			var db = Context;
			var userInfo = new UserInfo();
			userInfo.Id = Guid.NewGuid();
			userInfo.UserId = "blackstone";
			userInfo.Created = DateTime.Now;
			userInfo.Modified = DateTime.Now;
			db.Add(userInfo);
			await db.SaveChangesAsync();
		}

		public Task<string> AddMessage(XMessage message)
		{
			throw new NotImplementedException();
		}

		public Task<bool> AnyNews(string myId)
		{
			throw new NotImplementedException();
		}

		public Task<XIdentity> GetIdentityAsync(string identityId)
		{
			throw new NotImplementedException();
		}

		public Task<List<XMessage>> GetMessages(string myId)
		{
			throw new NotImplementedException();
		}
	}
}
