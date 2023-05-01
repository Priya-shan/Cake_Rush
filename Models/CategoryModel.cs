using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cake_Rush.Models
{
    public class CategoryModel
    {
        [Key]
        public int categoryId { get; set; }
        [Required]
        public string categoryName { get; set; }
    }
}
