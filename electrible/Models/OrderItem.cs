using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace electrible.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        [Required]
        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [ForeignKey("Product")]
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        // Navigation property to Order
        public virtual Order Order { get; set; }

        // Navigation property to Product
        public virtual Product Product { get; set; }
    }
}
