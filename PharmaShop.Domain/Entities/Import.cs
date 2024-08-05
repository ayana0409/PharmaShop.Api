using PharmaShop.Domain.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmaShop.Domain.Entities
{
    public class Import : BaseEntity
    {
        [Required]
        public DateTime ImportDate { get; set; }
        public double TotalCost { get; set; }
        [Required]
        public string SupplierId { get; set; } = string.Empty;
        [ForeignKey(nameof(SupplierId))]
        public ApplicationUser? Supplier { get; set; }
        public StatusProcessing Status { get; set; } = StatusProcessing.New;
        public ICollection<ImportDetail>? Details { get; set; }
    }
}
