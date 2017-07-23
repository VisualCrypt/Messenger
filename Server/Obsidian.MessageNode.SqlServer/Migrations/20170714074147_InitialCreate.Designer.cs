using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Obsidian.MessageNode.SqlServer.Models;

namespace Obsidian.MessageNode.SqlServer.Migrations
{
    [DbContext(typeof(InfoContext))]
    [Migration("20170714074147_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Obsidian.MessageNode.Models.MessageInfo", b =>
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

            modelBuilder.Entity("Obsidian.MessageNode.Models.UserInfo", b =>
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

            modelBuilder.Entity("Obsidian.MessageNode.Models.MessageInfo", b =>
                {
                    b.HasOne("Obsidian.MessageNode.Models.UserInfo", "UserInfo")
                        .WithMany("MessageInfos")
                        .HasForeignKey("UserInfoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
