namespace InventorySystem.Utils
{
    public static class SD
    {
        public const string Success = "Success";
        public const string Error = "Error";

        public const string ImagePath = @"\images\product\";
        public const string ssShoppingCarts = "Shopping Carts Session";

        public const string Role_Admin = "Admin";
        public const string Role_Client = "Client";
        public const string Role_Inventory = "Inventory";

        // Estados de la Orden
        public const string PendingStatus = "Pending";
        public const string ApprovedStatus = "Approved";
        public const string ProcessingStatus = "Processing";
        public const string CanceledStatus = "Canceled";
        public const string ReturnedStatus = "Returned";

        // Estado de pago de la Orden
        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelayed = "Delayed";
        public const string PaymentStatusRejected = "Rejected";
    }
}