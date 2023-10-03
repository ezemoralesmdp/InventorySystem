using InventorySystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem.DataAccess.Configuration
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(60);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Country).IsRequired();
            builder.Property(x => x.City).IsRequired();
            builder.Property(x => x.Address).IsRequired();
            builder.Property(x => x.Telephone).IsRequired();
            builder.Property(x => x.StoreSellId).IsRequired();
            builder.Property(x => x.CreatedById).IsRequired(false);
            builder.Property(x => x.UpdatedById).IsRequired(false);

            /* Relationships */

            builder.HasOne(x => x.Store).WithMany().HasForeignKey(x => x.StoreSellId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.CreatedBy).WithMany().HasForeignKey(x => x.CreatedById).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.UpdatedBy).WithMany().HasForeignKey(x => x.UpdatedById).OnDelete(DeleteBehavior.NoAction);
        }
    }
}