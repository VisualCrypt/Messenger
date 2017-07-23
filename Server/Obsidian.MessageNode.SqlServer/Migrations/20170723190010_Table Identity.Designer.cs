using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Obsidian.MessageNode.SqlServer.Models;

namespace Obsidian.MessageNode.SqlServer.Migrations
{
    [DbContext(typeof(InfoContext))]
    [Migration("20170723190010_Table Identity")]
    partial class TableIdentity
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Obsidian.MessageNode.SqlServer.Models.UserInfo", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("FirstSeenUTC");

                    b.Property<DateTime>("LastSeenUTC");

                    b.Property<string>("PublicKey");

                    b.HasKey("Id");

                    b.ToTable("UserInfos");
                });
        }
    }
}
