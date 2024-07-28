using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace electrible.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative integer.")]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, 10000)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Please select an image.")]
        public string ImageUrl { get; set; }

        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

    }
}

