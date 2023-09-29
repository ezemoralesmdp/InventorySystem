using InventorySystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem.DataAccess.Configuration
{
    public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.StoreId).IsRequired();
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.InitialDate).IsRequired();
            builder.Property(x => x.FinalDate).IsRequired();
            builder.Property(x => x.State).IsRequired();

            /* Relationships */

            builder.HasOne(x => x.Store).WithMany().HasForeignKey(x => x.StoreId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}