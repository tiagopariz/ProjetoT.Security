using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjetoT.Security.Domain.Entities.OAuth;

namespace ProjetoT.Security.Infra.Data.Mappings
{
    public class TokenMap : IEntityTypeConfiguration<Token>
    {
        public void Configure(EntityTypeBuilder<Token> builder)
        {
            #region Common mappings

            var tableSchema = typeof(Token).Namespace.Split('.').Last().ToLower();
            var tableName = typeof(Token).Name;

            builder.ToTable(tableName, tableSchema);

            builder.HasKey(x => x.Id)
                .HasName($"PK_{tableSchema}_{tableName}_Id");

            builder.Property(x => x.Id)
                .HasColumnName("Id");

            #endregion Common mappings

            builder.HasOne(x => x.User)
                .WithMany(y => y.UserTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}