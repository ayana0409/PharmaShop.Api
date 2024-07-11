using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PharmaShop.Infastructure.Entities
{
    public class ImportDetail 
    {
        [Required]
        public string? BatchNumber { get; set; }
        [Required]
        public DateTime ManufactureDate { get; set; }
        [Required]
        public int Expiry { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public double Cost { get; set; }

        [Required]
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }
        [Required]
        public int? ImportId { get; set; }
        [ForeignKey(nameof(ImportId))]
        public Import? Import { get; set; }
    }
}
