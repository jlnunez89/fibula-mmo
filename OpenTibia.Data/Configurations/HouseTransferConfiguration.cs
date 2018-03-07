namespace OpenTibia.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using OpenTibia.Data.Models;

    public class HouseTransferConfiguration : IEntityTypeConfiguration<HouseTransfer>
    {
        public void Configure(EntityTypeBuilder<HouseTransfer> builder)
        {
            builder.HasKey(b => b.Id);
            
            builder.Property(t => t.HouseId)
                .IsRequired();
        }
    }
}
