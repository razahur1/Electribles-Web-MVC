using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace electrible.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }

        [Required]
        [ForeignKey("Cart")]
        public int CartId { get; set; }

        public virtual Cart Cart { get; set; }

        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, 10000)]
        public decimal Price { get; set; } 
    }
}
