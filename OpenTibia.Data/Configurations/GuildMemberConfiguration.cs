namespace OpenTibia.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using OpenTibia.Data.Models;

    public class GuildMemberConfiguration : IEntityTypeConfiguration<GuildMember>
    {
        public void Configure(EntityTypeBuilder<GuildMember> builder)
        {
            builder.HasKey(b => b.EntryId);
            
            builder.Property(t => t.AccountId)
                .IsRequired();
            
            builder.Property(t => t.GuildId)
                .IsRequired();
        }
    }
}
