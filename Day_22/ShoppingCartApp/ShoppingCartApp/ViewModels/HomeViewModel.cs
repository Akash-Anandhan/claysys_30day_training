using System.Collections.Generic;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.ViewModels
{
    public class HomeViewModel
    {
        public List<Product> FeaturedProducts { get; set; } = new();
        public List<Product> BestSellers { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
    }
}