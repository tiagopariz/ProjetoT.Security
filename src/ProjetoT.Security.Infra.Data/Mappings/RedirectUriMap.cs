using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjetoT.Security.Domain.Entities;
using ProjetoT.Security.Domain.Entities.Identity;
using ProjetoT.Security.Domain.Entities.OAuth;

namespace ProjetoT.Security.Infra.Data.Mappings
{
    public class RedirectUriMap : IEntityTypeConfiguration<RedirectUri>
    {
        public void Configure(EntityTypeBuilder<RedirectUri> builder)
        {
            #region Common mappings

            var tableSchema = typeof(RedirectUri).Namespace.Split('.').Last().ToLower();
            var tableName = typeof(RedirectUri).Name;

            builder.ToTable(tableName, tableSchema);

            builder.HasKey(x => x.Id)
                .HasName($"PK_{tableSchema}_{tableName}_Id");

            builder.Property(x => x.Id)
                .HasColumnName("Id");

            #endregion Common mappings

            /* When an OAuth Client is deleted, delete any Redirect URIs it used. */
            builder.HasOne(x => x.ClientApplication)
                .WithMany(x => x.RedirectUris)
                .HasForeignKey(x => x.ClientApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}