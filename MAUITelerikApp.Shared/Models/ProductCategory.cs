using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.Shared.Models
{
    public class ProductCategory
    {
        [Key] // Primary Key
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Category name cannot exceed 50 characters.")]
        public string Name { get; set; }

        public string Image {  get; set; }

        // Navigation Property: A category can have multiple products
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
