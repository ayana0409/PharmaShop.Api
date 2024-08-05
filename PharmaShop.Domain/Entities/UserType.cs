using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PharmaShop.Domain.Entities
{
    public class UserType : BaseEntity
    {
        [Required]
        [StringLength(500)]
        public string? Name { get; set; }
        public double Discount { get; set; }
        public double MaxDiscount { get; set; }
        public int Point { get; set; }
        public bool IsActive { get; set; }
    }
}
