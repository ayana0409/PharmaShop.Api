using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PharmaShop.Infastructure.Entities
{
    public class ProductDetail : BaseEntity
    {
        [Required]
        [StringLength(500)]
        public string? Name { get; set; }
        [Required]
        [StringLength(2000)]
        public string? Content { get; set; }
        [Required]
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }
    }
}
