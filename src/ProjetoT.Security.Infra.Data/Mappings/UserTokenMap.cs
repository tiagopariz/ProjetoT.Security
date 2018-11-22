using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjetoT.Security.Domain.Entities.Identity;

namespace ProjetoT.Security.Infra.Data.Mappings
{
    public class UserTokenMap : IEntityTypeConfiguration<UserToken>
    {
        public void Configure(EntityTypeBuilder<UserToken> builder)
        {
            #region Common mappings

            var tableSchema = typeof(UserToken).Namespace.Split('.').Last().ToLower();
            var tableName = typeof(UserToken).Name;

            builder.ToTable(tableName, tableSchema);

            #endregion Common mappings
        }
    }
}