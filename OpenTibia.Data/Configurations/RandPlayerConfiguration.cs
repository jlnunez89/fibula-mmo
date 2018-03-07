namespace OpenTibia.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using OpenTibia.Data.Models;

    public class RandPlayerConfiguration : IEntityTypeConfiguration<RandPlayer>
    {
        public void Configure(EntityTypeBuilder<RandPlayer> builder)
        {
            builder.HasKey(b => b.RandId);
            
            builder.Property(t => t.AccountId)
                .IsRequired();
        }
    }
}
