using PharmaShop.Domain.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmaShop.Domain.Entities
{
    public class Order : BaseEntity
    {
        public DateTime OrderDate { get; set; }
        public double TotalPrice { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public StatusProcessing Status { get; set; }
        [Required]
        public string? CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public ApplicationUser? Customer { get; set; }
        public ICollection<OrderDetail>? Details { get; set; }
    }
}
