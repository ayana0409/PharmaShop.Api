using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmaShop.Infastructure.Entities
{
    public class OrderDetail
    {
        public int Quantity { get; set; }
        public double Price { get; set; }
        public bool IsUnit { get; set; }
        [Required]
        public int OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        public Order? Order { get; set; }
        [Required]
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }
    }
}
