namespace OpenTibia.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using OpenTibia.Data.Models;

    public class OnlinePlayerConfiguration : IEntityTypeConfiguration<OnlinePlayer>
    {
        public void Configure(EntityTypeBuilder<OnlinePlayer> builder)
        {
            builder.HasKey(b => b.Name);
            
            builder.Property(t => t.Level)
                .IsRequired();
            
            builder.Property(t => t.Vocation)
                .IsRequired();
        }
    }
}
