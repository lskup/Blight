// <auto-generated />
using System;
using Blight.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Blight.Migrations
{
    [DbContext(typeof(BlightDbContext))]
    [Migration("20220524124458_V1.0")]
    partial class V10
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.13")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Blight.Entieties.PhoneNumber", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("IsBullyTreshold")
                        .HasColumnType("int");

                    b.Property<string>("Number")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Prefix")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("PhoneNumbers");
                });

            modelBuilder.Entity("Blight.Entieties.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Master"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Admin"
                        },
                        new
                        {
                            Id = 3,
                            Name = "User"
                        });
                });

            modelBuilder.Entity("Blight.Entieties.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Banned")
                        .HasColumnType("bit");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nationality")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Banned = false,
                            DateOfBirth = new DateTime(1999, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "master@example.com",
                            FirstName = "Master",
                            LastName = "Master",
                            Nationality = "Poland",
                            Password = "AQAAAAEAACcQAAAAEIqVkVbKzIPnKViW//zCSfdAZkiGiU3e5sJ1ewbtsF966WFkhXOvNbHIFMYFgKzHpQ==",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 2,
                            Banned = false,
                            DateOfBirth = new DateTime(1999, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "admin@example.com",
                            FirstName = "Admin",
                            LastName = "Admin",
                            Nationality = "Poland",
                            Password = "AQAAAAEAACcQAAAAEN0vmYl6vhX0785CoKJcTWNVYm3qbIzERBcoGN9MOKp0BdWResQGTaVteQVNxHYhhA==",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 3,
                            Banned = false,
                            DateOfBirth = new DateTime(1999, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "user@example.com",
                            FirstName = "User",
                            LastName = "User",
                            Nationality = "Poland",
                            Password = "AQAAAAEAACcQAAAAEPX5W8qOzGNKA6fs58nDb2H/uTfY60LiQUGTUMf7Ixqd16olYt9XzXgvekjlh7RGZg==",
                            RoleId = 3
                        });
                });

            modelBuilder.Entity("PhoneNumberUser", b =>
                {
                    b.Property<int>("BlockedNumbersId")
                        .HasColumnType("int");

                    b.Property<int>("UsersId")
                        .HasColumnType("int");

                    b.HasKey("BlockedNumbersId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("PhoneNumberUser");
                });

            modelBuilder.Entity("Blight.Entieties.User", b =>
                {
                    b.HasOne("Blight.Entieties.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("PhoneNumberUser", b =>
                {
                    b.HasOne("Blight.Entieties.PhoneNumber", null)
                        .WithMany()
                        .HasForeignKey("BlockedNumbersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Blight.Entieties.User", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
