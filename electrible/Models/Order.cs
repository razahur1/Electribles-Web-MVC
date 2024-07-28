using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace electrible.Models
{
     public class Order
      {
            [Key]
            public int OrderId { get; set; }

            [Required]
            [ForeignKey("User")]
            public int UserId { get; set; }

            public DateTime OrderDate { get; set; }

            [Required]
            public decimal TotalAmount { get; set; }

            // New billing details fields
            [Required]
            public string Address { get; set; }

            [Required]
            //[RegularExpression(@"^(\+[0-9]{1,3})?[0-9]{10}$", ErrorMessage = "Invalid mobile number")]
            public string Mobile { get; set; }

            // Navigation property to User
            public virtual User User { get; set; }

            // Navigation property to OrderItems
            public virtual List<OrderItem> OrderItems { get; set; }
      }
 }
