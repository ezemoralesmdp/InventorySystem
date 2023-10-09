using InventorySystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem.DataAccess.Configuration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.OrderDate).IsRequired();
            builder.Property(x => x.TotalOrder).IsRequired();
            builder.Property(x => x.OrderState).IsRequired();
            builder.Property(x => x.PaymentState).IsRequired();
            builder.Property(x => x.ClientNames).IsRequired();
            builder.Property(x => x.DeliveryNumber).IsRequired(false);
            builder.Property(x => x.Carrier).IsRequired(false);
            builder.Property(x => x.TransactionId).IsRequired(false);
            builder.Property(x => x.Telephone).IsRequired(false);
            builder.Property(x => x.Address).IsRequired(false);
            builder.Property(x => x.City).IsRequired(false);
            builder.Property(x => x.Country).IsRequired(false);
            builder.Property(x => x.SessionId).IsRequired(false);

            /* Relationships */

            builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}