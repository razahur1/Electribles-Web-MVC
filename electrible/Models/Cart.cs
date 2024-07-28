using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace electrible.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<CartItem> CartItems { get; set; }

        [Required]
        [Range(0.01, 100000)]
        public decimal TotalAmount { get; set; }
    }
}
