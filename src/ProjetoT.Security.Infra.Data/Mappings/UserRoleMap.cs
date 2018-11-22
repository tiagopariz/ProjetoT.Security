using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjetoT.Security.Domain.Entities.Identity;

namespace ProjetoT.Security.Infra.Data.Mappings
{
    public class UserRoleMap : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            #region Common mappings

            var tableSchema = typeof(UserRole).Namespace.Split('.').Last().ToLower();
            var tableName = typeof(UserRole).Name;

            builder.ToTable(tableName, tableSchema);

            #endregion Common mappings
        }
    }
}