using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PharmaShop.Infastructure.Entities
{
    public class Image : BaseEntity
    {
        [Required]
        [StringLength(500)]
        public string? Path { get; set; }
        [Required]
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }
    }
}
