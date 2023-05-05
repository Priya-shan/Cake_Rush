namespace Cake_Rush.Models
{
    public class OrderDisplayModel
    {
        public int orderId { get; set; }
        public string userId { get; set; }
        public int cartId { get; set; }
        public CartModel cart { get; set; }
        public string message { get; set; }

        public int amount { get; set; }

        public string orderStatus { get; set; }

        public DateTime dateOrdered { get; set; }
        public string paymentMode { get; set; }

        public string deliveryMode { get; set; }

    }
}
