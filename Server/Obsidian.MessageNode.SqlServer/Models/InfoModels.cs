using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
	    public DbSet<MessageInfo> MessageInfos { get; set; }
	    public DbSet<Identity> Identities { get; set; }

	    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	    {
		    var conn = "Data Source=tcp:obsidianmessagenodedbserver.database.windows.net,1433;Initial Catalog=ObsidianMessageNode_db;User Id=blackstone@obsidianmessagenodedbserver;Password=!Black:Stone1777";

			 optionsBuilder.UseSqlServer(conn);
		}
		  
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

	public class Identity
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public DateTime LastSeenUTC { get; set; }
		public DateTime FirstSeenUTC;
		public byte[] PublicIdentityKey { get; set; }
		public int ContactState { get; set; }
	}

}
