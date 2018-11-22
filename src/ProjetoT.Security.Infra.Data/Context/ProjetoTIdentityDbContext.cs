using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjetoT.Security.Domain.Entities;
using ProjetoT.Security.Domain.Entities.Identity;
using ProjetoT.Security.Domain.Entities.OAuth;
using ProjetoT.Security.Infra.Data.Mappings;

namespace ProjetoT.Security.Infra.Data.Context
{
    public class ProjetoTIdentityDbContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        public DbSet<ClientApplication> ClientApplications { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<RedirectUri> RedirectUris { get; set; }
        public DbSet<RateLimit> RateLimits { get; set; }

        public ProjetoTIdentityDbContext(DbContextOptions<ProjetoTIdentityDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Identity
            modelBuilder.ApplyConfiguration(new UserMap());
            modelBuilder.ApplyConfiguration(new RoleMap());
            modelBuilder.ApplyConfiguration(new UserClaimMap());
            modelBuilder.ApplyConfiguration(new UserTokenMap());
            modelBuilder.ApplyConfiguration(new UserLoginMap());
            modelBuilder.ApplyConfiguration(new RoleClaimMap());
            modelBuilder.ApplyConfiguration(new UserRoleMap());
            // OAuth
            modelBuilder.ApplyConfiguration(new ClientApplicationMap());
            modelBuilder.ApplyConfiguration(new RedirectUriMap());
            modelBuilder.ApplyConfiguration(new TokenMap());
            modelBuilder.ApplyConfiguration(new RateLimitMap());

            // JWT
        }
    }
}