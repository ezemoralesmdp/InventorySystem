using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystem.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime DeliveryDate { get; set; }

        public string DeliveryNumber { get; set; }

        public string Carrier { get; set; }

        [Required]
        public double TotalOrder { get; set; }

        [Required]
        public string OrderState { get; set; }

        public string PaymentState { get; set; }

        public DateTime PaymentDate { get; set; }

        public DateTime MaxPaymentDate { get; set; }

        public string TransactionId { get; set; }

        public string SessionId { get; set; }

        public string Telephone { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string ClientNames { get; set; }
    }
}