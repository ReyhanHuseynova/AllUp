using _allup.Areas.admin.Controllers;
using System.ComponentModel.DataAnnotations.Schema;

namespace _allup.Models
{
    public class Product
    {
        public int Id { get; set; }
        
        public string Rate { get; set; }
        public double Price { get; set; }
         public string Name { get; set; }

        public bool IsDeactive { get; set; }
        public Brand Brand { get; set; }
        public int BrandId { get; set; }
        public List<ProductTag> ProductTags { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        
        public ProductDetail ProductDetails { get; set; }
       
        [NotMapped]
        public IFormFile[] Photos { get; set; }
    }
}
