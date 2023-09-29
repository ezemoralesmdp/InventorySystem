using InventorySystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem.DataAccess.Configuration
{
    public class InventoryDetailConfiguration : IEntityTypeConfiguration<InventoryDetail>
    {
        public void Configure(EntityTypeBuilder<InventoryDetail> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.InventoryId).IsRequired();
            builder.Property(x => x.ProductId).IsRequired();
            builder.Property(x => x.PreviousStock).IsRequired();
            builder.Property(x => x.Amount).IsRequired();

            /* Relationships */

            builder.HasOne(x => x.Inventory).WithMany().HasForeignKey(x => x.InventoryId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}