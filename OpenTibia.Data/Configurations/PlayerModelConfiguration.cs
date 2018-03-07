namespace OpenTibia.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using OpenTibia.Data.Models;

    public class PlayerModelConfiguration : IEntityTypeConfiguration<PlayerModel>
    {
        public void Configure(EntityTypeBuilder<PlayerModel> builder)
        {
            builder.HasKey(b => b.Player_Id);
            
            builder.Property(t => t.Charname)
                .IsRequired();

            builder.Property(t => t.Account_Id)
                .IsRequired();

            builder.Property(t => t.Account_Nr)
                .IsRequired();
        }
    }
}
