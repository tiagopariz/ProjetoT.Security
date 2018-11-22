﻿using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjetoT.Security.Domain.Entities.Identity;

namespace ProjetoT.Security.Infra.Data.Mappings
{
    public class UserLoginMap : IEntityTypeConfiguration<UserLogin>
    {
        public void Configure(EntityTypeBuilder<UserLogin> builder)
        {
            #region Common mappings

            var tableSchema = typeof(UserLogin).Namespace.Split('.').Last().ToLower();
            var tableName = typeof(UserLogin).Name;

            builder.ToTable(tableName, tableSchema);

            //builder.HasKey(x => x.Id)
            //    .HasName($"PK_{tableSchema}_{tableName}_Id");

            //builder.Property(x => x.Id)
            //    .HasColumnName("Id");

            #endregion Common mappings
        }
    }
}