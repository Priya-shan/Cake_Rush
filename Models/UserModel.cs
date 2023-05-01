using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cake_Rush.Models
{
    public class UserModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string userId { get; set; }

        public string? userName { get; set; }

        public string? email { get; set; }

        public string? mobile { get; set; }

        public string? address { get; set; }

        public string? city { get; set; }
        public string? pincode { get; set; }


    }
}
