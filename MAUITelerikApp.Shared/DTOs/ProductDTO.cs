using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.Shared.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; } // For returning category name

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public string Image { get; set; }

        public string Description { get; set; }
    }

}
