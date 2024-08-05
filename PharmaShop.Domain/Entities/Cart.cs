using System.ComponentModel.DataAnnotations;
namespace PharmaShop.Domain.Entities
{
    public class Cart : BaseEntity
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        public ApplicationUser? User { get; set; }
    }
}
