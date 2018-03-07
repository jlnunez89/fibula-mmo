namespace OpenTibia.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using OpenTibia.Data.Models;

    public class AssignedHouseConfiguration : IEntityTypeConfiguration<AssignedHouse>
    {
        public void Configure(EntityTypeBuilder<AssignedHouse> builder)
        {
            builder.HasKey(b => b.HouseId);
            
            builder.Property(t => t.PlayerId)
                .IsRequired();
            
            builder.Property(t => t.OwnerString)
                .IsRequired();

            builder.Property(t => t.Virgin)
                .IsRequired();

            builder.Property(t => t.Gold)
                .IsRequired();

            builder.Property(t => t.World)
                .IsRequired();
        }
    }
}
