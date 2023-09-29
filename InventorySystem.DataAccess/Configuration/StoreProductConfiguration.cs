using InventorySystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem.DataAccess.Configuration
{
    public class StoreProductConfiguration : IEntityTypeConfiguration<StoreProduct>
    {
        public void Configure(EntityTypeBuilder<StoreProduct> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.StoreId).IsRequired();   
            builder.Property(x => x.ProductId).IsRequired();
            builder.Property(x => x.Amount).IsRequired();

            /* Relationships */

            builder.HasOne(x => x.Store).WithMany().HasForeignKey(x => x.StoreId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}