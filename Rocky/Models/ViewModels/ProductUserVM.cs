using System.Collections.Generic;

namespace Rocky.Models.ViewModels
{
    public class ProductUserVM
    {
        public ApplicationUser ApplicationUser { get; set; }
        public List<Product> ProductList { get; set; } = new List<Product>();
    }
}
