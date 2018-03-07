namespace OpenTibia.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using OpenTibia.Data.Models;

    public class GuildConfiguration : IEntityTypeConfiguration<Guild>
    {
        public void Configure(EntityTypeBuilder<Guild> builder)
        {
            builder.HasKey(b => b.GuildId);
            
            builder.Property(t => t.GuildName)
                .IsRequired();
            
            builder.Property(t => t.GuildOwner)
                .IsRequired();
        }
    }
}
