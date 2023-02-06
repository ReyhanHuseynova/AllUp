using System.ComponentModel.DataAnnotations.Schema;

namespace _allup.Models
{
    public class ProductDetail
    {
        public int Id { get; set; }
        public int Tax { get; set; }
        public bool HasStock { get; set; }
        public Product Product { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public string Description { get; set; }
    }
}
