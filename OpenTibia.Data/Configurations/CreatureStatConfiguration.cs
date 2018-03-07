namespace OpenTibia.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using OpenTibia.Data.Models;

    public class CreatureStatConfiguration : IEntityTypeConfiguration<CreatureStat>
    {
        public void Configure(EntityTypeBuilder<CreatureStat> builder)
        {
            builder.HasKey(b => new { b.Name, b.Time });
            
            builder.Property(t => t.KilledBy)
                .IsRequired();
            
            builder.Property(t => t.Killed)
                .IsRequired();
        }
    }
}
