using InventorySystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem.DataAccess.Configuration
{
    public class KardexInventoryConfiguration : IEntityTypeConfiguration<KardexInventory>
    {
        public void Configure(EntityTypeBuilder<KardexInventory> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.StoreProductId).IsRequired();
            builder.Property(x => x.Type).IsRequired();
            builder.Property(x => x.Detail).IsRequired();
            builder.Property(x => x.PreviousStock).IsRequired();
            builder.Property(x => x.Amount).IsRequired();
            builder.Property(x => x.Cost).IsRequired();
            builder.Property(x => x.Stock).IsRequired();
            builder.Property(x => x.Total).IsRequired();
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.RegistrationDate).IsRequired();

            /* Relationships */

            builder.HasOne(x => x.StoreProduct).WithMany().HasForeignKey(x => x.StoreProductId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}