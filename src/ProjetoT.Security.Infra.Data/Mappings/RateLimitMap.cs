using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjetoT.Security.Domain.Entities.OAuth;

namespace ProjetoT.Security.Infra.Data.Mappings
{
    public class RateLimitMap : IEntityTypeConfiguration<RateLimit>
    {
        public void Configure(EntityTypeBuilder<RateLimit> builder)
        {
            #region Common mappings

            var tableSchema = typeof(RateLimit).Namespace.Split('.').Last().ToLower();
            var tableName = typeof(RateLimit).Name;

            builder.ToTable(tableName, tableSchema);

            builder.HasKey(x => x.Id)
                .HasName($"PK_{tableSchema}_{tableName}_Id");

            builder.Property(x => x.Id)
                .HasColumnName("Id");

            #endregion Common mappings

            /* RWhen a Rate Limit is deleted, delete any Tokens that use this rate limit */
            builder.HasOne(x => x.Token)
                .WithOne(x => x.RateLimit)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}