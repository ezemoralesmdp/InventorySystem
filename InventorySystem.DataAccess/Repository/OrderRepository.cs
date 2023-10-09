using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Models;

namespace InventorySystem.DataAccess.Repository
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Order order)
        {
            _db.Update(order);
            _db.SaveChanges();
        }

        public void UpdateState(int id, string orderState, string paymentState)
        {
            var orderDB = _db.Order.FirstOrDefault(o => o.Id == id);
            
            if(orderDB != null)
            {
                orderDB.OrderState = orderState;
                orderDB.PaymentState = paymentState;
            }
        }

        public void UpdatePaymentStripeId(int id, string sessionId, string transactionId)
        {
            var orderDB = _db.Order.FirstOrDefault(o => o.Id == id);

            if (orderDB != null)
            {
                if (!string.IsNullOrEmpty(sessionId))
                    orderDB.SessionId = sessionId;

                if (!string.IsNullOrEmpty(transactionId))
                {
                    orderDB.TransactionId = transactionId;
                    orderDB.PaymentDate = DateTime.Now;
                }
            }
        }
    }
}