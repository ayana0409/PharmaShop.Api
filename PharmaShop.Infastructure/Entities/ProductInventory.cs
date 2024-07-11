using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmaShop.Infastructure.Entities
{
    public class ProductInventory : BaseEntity
    {
        [Required]
        public string? BatchNumber { get; set; }
        [Required]
        public DateTime ManufactureDate { get; set; }
        [Required]
        public int Expiry { get; set; }
        public int BigUnitQuantity { get; set; }
        public int MediumUnitQuantity { get; set; }
        public int SmallUnitQuantity { get; set; }
        [Required]
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }
    }
}
