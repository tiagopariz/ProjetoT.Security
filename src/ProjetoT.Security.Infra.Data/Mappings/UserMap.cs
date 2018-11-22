using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjetoT.Security.Domain.Entities.Identity;

namespace ProjetoT.Security.Infra.Data.Mappings
{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            #region Common mappings

            var tableSchema = typeof(User).Namespace.Split('.').Last().ToLower();
            var tableName = typeof(User).Name;

            builder.ToTable(tableName, tableSchema);

            builder.HasKey(x => x.Id)
                .HasName($"PK_{tableSchema}_{tableName}_Id");

            builder.Property(x => x.Id)
                .HasColumnName("Id");

            #endregion Common mappings

            /* When an AspNetUser is deleted, delete their tokens */
            builder.HasMany(x => x.UserTokens)
                .WithOne(y => y.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}