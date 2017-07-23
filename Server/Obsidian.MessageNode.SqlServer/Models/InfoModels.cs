using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Obsidian.MessageNode.SqlServer.Models
{
    public class InfoContext : DbContext
    {
		public InfoContext() { }
	    public InfoContext(DbContextOptions<InfoContext> options)
		    : base(options)
	    { }
		public DbSet<UserInfo> UserInfos { get; set; }
	    public DbSet<Message> Messages { get; set; }
		// public DbSet<MessageInfo> MessageInfos { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	    {
		    var conn = "Data Source=tcp:obsidianmessagenodedbserver.database.windows.net,1433;Initial Catalog=ObsidianMessageNode_db;User Id=blackstone@obsidianmessagenodedbserver;Password=!Black:Stone1777";

			 optionsBuilder.UseSqlServer(conn);
		}
		  
	}

	public class UserInfo
	{
		public string Id { get; set; }
		public string PublicKey { get; set; }
		public DateTime FirstSeenUTC { get; set; }
		public DateTime LastSeenUTC { get; set; }
		public List<Message> MessageInfos { get; set; }
	}
	public class Message
	{
		public Guid Id { get; set; }
		public string XMessageId { get; set; }
		public string SenderId { get; set; }
		public string RecipientId { get; set; }
		public int MessageType { get; set; }
		public DateTime EncryptedDateUtc { get; set; }
		public byte[] TextCipher { get; set; }
		public byte[] ImageCipher { get; set; }
		public byte[] DynamicPublicKey { get; set; }
		public long DynamicPublicKeyId { get; set; }
		public long PrivateKeyHint { get; set; }

		// FK
		public string UserInfoId { get; set; }
		public UserInfo UserInfo { get; set; }

	}

	//public class MessageInfo
	//{
	//	public Guid Id { get; set; }

	//	public string MessageId { get; set; }
	//	public string SenderId { get; set; }
	//	public string RecipientId { get; set; }

	//	// FK
	//	public Guid UserInfoId { get; set; }
	//	public UserInfo UserInfo { get; set; }
	//}



}
