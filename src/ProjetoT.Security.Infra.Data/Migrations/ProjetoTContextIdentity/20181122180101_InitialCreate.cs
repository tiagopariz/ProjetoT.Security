using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjetoT.Security.Infra.Data.Migrations.ProjetoTContextIdentity
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "identity");

            migrationBuilder.EnsureSchema(
                name: "oauth");

            migrationBuilder.CreateTable(
                name: "Role",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_identity_Role_Id", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_identity_User_Id", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaim",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<Guid>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_identity_RoleClaim_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaim_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "identity",
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaim",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<Guid>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_identity_UserClaim_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaim_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogin",
                schema: "identity",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogin", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogin_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                schema: "identity",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    RoleId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRole_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "identity",
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRole_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserToken",
                schema: "identity",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserToken", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserToken_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientApplication",
                schema: "oauth",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ClientSecret = table.Column<string>(nullable: false),
                    ClientName = table.Column<string>(maxLength: 100, nullable: false),
                    ClientDescription = table.Column<string>(maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_oauth_ClientApplication_Id", x => x.Id);
                    table.UniqueConstraint("AK_ClientApplication_ClientName", x => x.ClientName);
                    table.ForeignKey(
                        name: "FK_ClientApplication_User_Id",
                        column: x => x.Id,
                        principalSchema: "identity",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RedirectUri",
                schema: "oauth",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ClientApplicationId = table.Column<Guid>(nullable: false),
                    URI = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_oauth_RedirectUri_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RedirectUri_ClientApplication_ClientApplicationId",
                        column: x => x.ClientApplicationId,
                        principalSchema: "oauth",
                        principalTable: "ClientApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Token",
                schema: "oauth",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    GrantType = table.Column<string>(nullable: true),
                    TokenType = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    ClientApplicationId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_oauth_Token_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Token_ClientApplication_ClientApplicationId",
                        column: x => x.ClientApplicationId,
                        principalSchema: "oauth",
                        principalTable: "ClientApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Token_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RateLimit",
                schema: "oauth",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Limit = table.Column<int>(nullable: true),
                    Window = table.Column<TimeSpan>(nullable: true),
                    TokenId = table.Column<Guid>(nullable: false),
                    ClientId = table.Column<Guid>(nullable: false),
                    SubordinatedClientId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_oauth_RateLimit_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RateLimit_ClientApplication_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "oauth",
                        principalTable: "ClientApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RateLimit_ClientApplication_SubordinatedClientId",
                        column: x => x.SubordinatedClientId,
                        principalSchema: "oauth",
                        principalTable: "ClientApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RateLimit_Token_TokenId",
                        column: x => x.TokenId,
                        principalSchema: "oauth",
                        principalTable: "Token",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "identity",
                table: "Role",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaim_RoleId",
                schema: "identity",
                table: "RoleClaim",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "identity",
                table: "User",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "identity",
                table: "User",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaim_UserId",
                schema: "identity",
                table: "UserClaim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogin_UserId",
                schema: "identity",
                table: "UserLogin",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId",
                schema: "identity",
                table: "UserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RateLimit_ClientId",
                schema: "oauth",
                table: "RateLimit",
                column: "ClientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RateLimit_SubordinatedClientId",
                schema: "oauth",
                table: "RateLimit",
                column: "SubordinatedClientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RateLimit_TokenId",
                schema: "oauth",
                table: "RateLimit",
                column: "TokenId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RedirectUri_ClientApplicationId",
                schema: "oauth",
                table: "RedirectUri",
                column: "ClientApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Token_ClientApplicationId",
                schema: "oauth",
                table: "Token",
                column: "ClientApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Token_UserId",
                schema: "oauth",
                table: "Token",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleClaim",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserClaim",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserLogin",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserRole",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserToken",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "RateLimit",
                schema: "oauth");

            migrationBuilder.DropTable(
                name: "RedirectUri",
                schema: "oauth");

            migrationBuilder.DropTable(
                name: "Role",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Token",
                schema: "oauth");

            migrationBuilder.DropTable(
                name: "ClientApplication",
                schema: "oauth");

            migrationBuilder.DropTable(
                name: "User",
                schema: "identity");
        }
    }
}
