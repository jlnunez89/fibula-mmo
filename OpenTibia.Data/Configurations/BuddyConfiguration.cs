namespace OpenTibia.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using OpenTibia.Data.Models;

    public class BuddyConfiguration : IEntityTypeConfiguration<Buddy>
    {
        public void Configure(EntityTypeBuilder<Buddy> builder)
        {
            builder.HasKey(b => b.EntryId);
            
            builder.Property(t => t.AccountNr)
                .IsRequired();

            builder.Property(t => t.BuddyId)
                .IsRequired();

            builder.Property(t => t.BuddyString)
                .IsRequired();

            builder.Property(t => t.Timestamp)
                .IsRequired();

            builder.Property(t => t.InitiatingId)
                .IsRequired();
        }
    }
}
