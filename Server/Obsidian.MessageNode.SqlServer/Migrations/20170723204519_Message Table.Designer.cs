using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Obsidian.MessageNode.SqlServer.Models;

namespace Obsidian.MessageNode.SqlServer.Migrations
{
    [DbContext(typeof(InfoContext))]
    [Migration("20170723204519_Message Table")]
    partial class MessageTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Obsidian.MessageNode.SqlServer.Models.Message", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("DynamicPublicKey");

                    b.Property<long>("DynamicPublicKeyId");

                    b.Property<DateTime>("EncryptedDateUtc");

                    b.Property<byte[]>("ImageCipher");

                    b.Property<int>("MessageType");

                    b.Property<long>("PrivateKeyHint");

                    b.Property<string>("RecipientId");

                    b.Property<string>("SenderId");

                    b.Property<byte[]>("TextCipher");

                    b.Property<string>("UserInfoId");

                    b.Property<string>("XMessageId");

                    b.HasKey("Id");

                    b.HasIndex("UserInfoId");

                    b.ToTable("Messages");
                });

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

            modelBuilder.Entity("Obsidian.MessageNode.SqlServer.Models.Message", b =>
                {
                    b.HasOne("Obsidian.MessageNode.SqlServer.Models.UserInfo", "UserInfo")
                        .WithMany("MessageInfos")
                        .HasForeignKey("UserInfoId");
                });
        }
    }
}
