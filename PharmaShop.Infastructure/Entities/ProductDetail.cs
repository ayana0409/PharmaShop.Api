using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PharmaShop.Infastructure.Entities
{
    public class ProductDetail : BaseEntity
    {
        [Required]
        [StringLength(500)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [StringLength(5000)]
        public string Content { get; set; } = string.Empty;
        [Required]
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }
    }
}
