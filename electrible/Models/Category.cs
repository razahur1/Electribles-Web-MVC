using electrible.Models;
using System.ComponentModel.DataAnnotations;

namespace electrible.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
