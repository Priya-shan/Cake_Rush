namespace Cake_Rush.Models
{
    public class CartContentModel
    {
        public string userId { get; set; }
        public int cartId { get; set; }
        public string productName { get; set; }
        public string productDescription { get; set; }
        public string imageid { get; set; }
        public string label { get; set; }
        public int quantity { get; set; }
        public int totalPrice { get; set; }
    }
}
