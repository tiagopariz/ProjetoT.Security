using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjetoT.Security.Domain.Entities.OAuth;

namespace ProjetoT.Security.Infra.Data.Mappings
{
    public class ClientApplicationMap : IEntityTypeConfiguration<ClientApplication>
    {
        public void Configure(EntityTypeBuilder<ClientApplication> builder)
        {
            #region Common mappings

            var tableSchema = typeof(ClientApplication).Namespace.Split('.').Last().ToLower();
            var tableName = typeof(ClientApplication).Name;

            builder.ToTable(tableName, tableSchema);

            builder.HasKey(x => x.Id)
                .HasName($"PK_{tableSchema}_{tableName}_Id");

            builder.Property(x => x.Id)
                .HasColumnName("Id");

            #endregion Common mappings

            /*An ClientsApplications name is unique among all other ClientsApplications */
            builder.HasAlternateKey(x => x.ClientName);

            /* When an AspNet User is deleted, delete their created ClientsApplications */
            builder.HasOne(x => x.Owner)
                .WithMany(x => x.ClientsApplications)
                .OnDelete(DeleteBehavior.Cascade);

            /* When an OAuth Client is deleted, delete any tokens it issued */
            builder.HasMany(x => x.UserTokens)
                .WithOne(x => x.Client)
                .HasForeignKey(x => x.ClientApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            /* When an OAuthClient is deleted, delete its Rate Limits */
            builder.HasOne(x => x.RateLimit)
                .WithOne(x => x.Client)
                .HasForeignKey<RateLimit>(x => x.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            /* When an OAuthClient is deleted, delete its Subordinate Rate Limit */
            builder.HasOne(x => x.SubordinateTokenLimits)
                .WithOne(x => x.SubordinatedClient)
                .HasForeignKey<RateLimit>(x => x.SubordinatedClientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}