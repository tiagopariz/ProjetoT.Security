﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProjetoT.Security.Infra.Data.Context;

namespace ProjetoT.Security.Infra.Data.Migrations.ProjetoTContextIdentity
{
    [DbContext(typeof(ProjetoTIdentityDbContext))]
    partial class ProjetoTIdentityDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ProjetoT.Security.Domain.Entities.Identity.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id")
                        .HasName("PK_identity_Role_Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("Role","identity");
                });

            modelBuilder.Entity("ProjetoT.Security.Domain.Entities.Identity.RoleClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<Guid>("RoleId");

                    b.HasKey("Id")
                        .HasName("PK_identity_RoleClaim_Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleClaim","identity");
                });

            modelBuilder.Entity("ProjetoT.Security.Domain.Entities.Identity.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id")
                        .HasName("PK_identity_User_Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("User","identity");
                });

            modelBuilder.Entity("ProjetoT.Security.Domain.Entities.Identity.UserClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id")
                        .HasName("PK_identity_UserClaim_Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserClaim","identity");
                });

            modelBuilder.Entity("ProjetoT.Security.Domain.Entities.Identity.UserLogin", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<Guid>("UserId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogin","identity");
                });

            modelBuilder.Entity("ProjetoT.Security.Domain.Entities.Identity.UserRole", b =>
                {
                    b.Property<Guid>("UserId");

                    b.Property<Guid>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRole","identity");
                });

            modelBuilder.Entity("ProjetoT.Security.Domain.Entities.Identity.UserToken", b =>
                {
                    b.Property<Guid>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("UserToken","identity");
                });

            modelBuilder.Entity("ProjetoT.Security.Domain.Entities.OAuth.ClientApplication", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnName("Id");

                    b.Property<string>("ClientDescription")
                        .IsRequired()
                        .HasMaxLength(300);

                    b.Property<string>("ClientName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("ClientSecret")
                        .IsRequired();

                    b.HasKey("Id")
                        .HasName("PK_oauth_ClientApplication_Id");

                    b.HasAlternateKey("ClientName");

                    b.ToTable("ClientApplication","oauth");
                });

            modelBuilder.Entity("ProjetoT.Security.Domain.Entities.OAuth.RateLimit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id");

                    b.Property<Guid>("ClientId");

                    b.Property<int?>("Limit");

                    b.Property<Guid>("SubordinatedClientId");

                    b.Property<Guid>("TokenId");

                    b.Property<TimeSpan?>("Window");

                    b.HasKey("Id")
                        .HasName("PK_oauth_RateLimit_Id");

                    b.HasIndex("ClientId")
                        .IsUnique();

                    b.HasIndex("SubordinatedClientId")
                        .IsUnique();

                    b.HasIndex("TokenId")
                        .IsUnique();

                    b.ToTable("RateLimit","oauth");
                });

            modelBuilder.Entity("ProjetoT.Security.Domain.Entities.OAuth.RedirectUri", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id");

                    b.Property<Guid>("ClientApplicationId");

                    b.Property<string>("URI");

                    b.HasKey("Id")
                        .HasName("PK_oauth_RedirectUri_Id");

                    b.HasIndex("ClientApplicationId");

                    b.ToTable("RedirectUri","oauth");
                });

            modelBuilder.Entity("ProjetoT.Security.Domain.Entities.OAuth.Token", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id");

                    b.Property<Guid>("ClientApplicationId");

                    b.Property<string>("GrantType");

                    b.Property<string>("TokenType");

                    b.Property<Guid>("UserId");

                    b.Property<string>("Value");

                    b.HasKey("Id")
                        .HasName("PK_oauth_Token_Id");

                    b.HasIndex("ClientApplicationId");

                    b.HasIndex("UserId");

                    b.ToTable("Token","oauth");
                });

            modelBuilder.Entity("ProjetoT.Security.Domain.Entities.Identity.RoleClaim", b =>
                {
                    b.HasOne("ProjetoT.Security.Domain.Entities.Identity.Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ProjetoT.Security.Domain.Entities.Identity.UserClaim", b =>
                {
                    b.HasOne("ProjetoT.Security.Domain.Entities.Identity.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ProjetoT.Security.Domain.Entities.Identity.UserLogin", b =>
                {
                    b.HasOne("ProjetoT.Security.Domain.Entities.Identity.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ProjetoT.Security.Domain.Entities.Identity.UserRole", b =>
                {
                    b.HasOne("ProjetoT.Security.Domain.Entities.Identity.Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ProjetoT.Security.Domain.Entities.Identity.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ProjetoT.Security.Domain.Entities.Identity.UserToken", b =>
                {
                    b.HasOne("ProjetoT.Security.Domain.Entities.Identity.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ProjetoT.Security.Domain.Entities.OAuth.ClientApplication", b =>
                {
                    b.HasOne("ProjetoT.Security.Domain.Entities.Identity.User", "Owner")
                        .WithMany("ClientsApplications")
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ProjetoT.Security.Domain.Entities.OAuth.RateLimit", b =>
                {
                    b.HasOne("ProjetoT.Security.Domain.Entities.OAuth.ClientApplication", "Client")
                        .WithOne("RateLimit")
                        .HasForeignKey("ProjetoT.Security.Domain.Entities.OAuth.RateLimit", "ClientId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ProjetoT.Security.Domain.Entities.OAuth.ClientApplication", "SubordinatedClient")
                        .WithOne("SubordinateTokenLimits")
                        .HasForeignKey("ProjetoT.Security.Domain.Entities.OAuth.RateLimit", "SubordinatedClientId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("ProjetoT.Security.Domain.Entities.OAuth.Token", "Token")
                        .WithOne("RateLimit")
                        .HasForeignKey("ProjetoT.Security.Domain.Entities.OAuth.RateLimit", "TokenId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("ProjetoT.Security.Domain.Entities.OAuth.RedirectUri", b =>
                {
                    b.HasOne("ProjetoT.Security.Domain.Entities.OAuth.ClientApplication", "ClientApplication")
                        .WithMany("RedirectUris")
                        .HasForeignKey("ClientApplicationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ProjetoT.Security.Domain.Entities.OAuth.Token", b =>
                {
                    b.HasOne("ProjetoT.Security.Domain.Entities.OAuth.ClientApplication", "Client")
                        .WithMany("UserTokens")
                        .HasForeignKey("ClientApplicationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ProjetoT.Security.Domain.Entities.Identity.User", "User")
                        .WithMany("UserTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}