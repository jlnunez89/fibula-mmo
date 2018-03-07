namespace OpenTibia.Data.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using OpenTibia.Data.Models;

    public class BanishmentConfiguration : IEntityTypeConfiguration<Banishment>
    {
        public void Configure(EntityTypeBuilder<Banishment> builder)
        {
            builder.HasKey(b => b.BanishmentId);

            builder.Property(b => b.BanishmentId)
                .ValueGeneratedOnAdd();

            builder.Property(t => t.AccountNr)
                .IsRequired();

            builder.Property(t => t.AccountId)
                .IsRequired();
            
            builder.Property(t => t.Violation)
                .IsRequired();

            builder.Property(t => t.Timestamp)
                .IsRequired();

            builder.Property(t => t.BanishedUntil)
                .IsRequired();

            builder.Property(t => t.GmId)
                .IsRequired();

            builder.Property(t => t.PunishmentType)
                .IsRequired();
        }
    }
}
