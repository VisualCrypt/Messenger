using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Obsidian.MessageNode.SqlServer.Models;

namespace Obsidian.MessageNode.SqlServer.Migrations
{
    [DbContext(typeof(InfoContext))]
    [Migration("20170722190932_Identity Table")]
    partial class IdentityTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Obsidian.MessageNode.SqlServer.Models.Identity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ContactState");

                    b.Property<DateTime>("LastSeenUTC");

                    b.Property<string>("Name");

                    b.Property<byte[]>("PublicIdentityKey");

                    b.HasKey("Id");

                    b.ToTable("Identities");
                });

            modelBuilder.Entity("Obsidian.MessageNode.SqlServer.Models.MessageInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("MessageId");

                    b.Property<string>("RecipientId");

                    b.Property<string>("SenderId");

                    b.Property<Guid>("UserInfoId");

                    b.HasKey("Id");

                    b.HasIndex("UserInfoId");

                    b.ToTable("MessageInfos");
                });

            modelBuilder.Entity("Obsidian.MessageNode.SqlServer.Models.UserInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("PublicKey");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("UserInfos");
                });

            modelBuilder.Entity("Obsidian.MessageNode.SqlServer.Models.MessageInfo", b =>
                {
                    b.HasOne("Obsidian.MessageNode.SqlServer.Models.UserInfo", "UserInfo")
                        .WithMany("MessageInfos")
                        .HasForeignKey("UserInfoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
