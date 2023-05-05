namespace Cake_Rush.Models
{
    public class SubCategory
    {
        public int mapId { get; set; }
        public int productId { get; set; }
        public string categoryName { get; set; }
        public int price { get; set; }

        public ProductModel Product { get; set; }
    }
}
