namespace Cake_Rush.Models
{
    public class CartModel
    {
        public int cartId { get; set; }
        public string userId { get; set; }
        public int mapId { get; set; }
        public int quantity { get; set; }
        public int price { get; set; }
        public int expiry { get; set; }

        public UserModel User { get; set; }
        public SubCategory SubCatMap { get; set;}
    }
}
