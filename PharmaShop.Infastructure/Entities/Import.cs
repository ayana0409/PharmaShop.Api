using PharmaShop.Infastructure.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmaShop.Infastructure.Entities
{
    public class Import : BaseEntity
    {
        [Required]
        public DateTime ImportDate { get; set; }
        public double totalCost { get; set; }
        [Required]
        public string? ProviderId { get; set; }
        [ForeignKey(nameof(ProviderId))]
        public ApplicationUser? Provider { get; set; }
        public ICollection<ImportDetail>? Details { get; set; }
    }
}
