using InventorySystem.Models;

namespace InventorySystem.DataAccess.Repository.IRepository
{
    public interface IOrderRepository : IRepository<Order>
    {
        void Update(Order order);
        void UpdateState(int id, string orderState, string paymentState);
        void UpdatePaymentStripeId(int id, string sessionId, string transactionId);
    }
}