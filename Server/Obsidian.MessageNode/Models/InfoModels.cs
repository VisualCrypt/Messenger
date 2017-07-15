using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Obsidian.MessageNode.Models
{
    public class InfoContext : DbContext
    {
	    public InfoContext(DbContextOptions<InfoContext> options)
		    : base(options)
	    { }
		public DbSet<UserInfo> UserInfos { get; set; }
	    public DbSet<MessageInfo> MessageInfos { get; set; }
	}

	public class UserInfo
	{
		public Guid Id { get; set; }
		public string UserId { get; set; }
		public string PublicKey { get; set; }
		public DateTime Created { get; set; }
		public DateTime Modified { get; set; }
		public List<MessageInfo> MessageInfos { get; set; }
	}

	public class MessageInfo
	{
		public Guid Id { get; set; }

		public string MessageId { get; set; }
		public string SenderId { get; set; }
		public string RecipientId { get; set; }

		// FK
		public Guid UserInfoId { get; set; }
		public UserInfo UserInfo { get; set; }
	}

}
