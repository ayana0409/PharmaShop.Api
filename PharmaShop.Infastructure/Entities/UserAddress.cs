using PharmaShop.Infastructure.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PharmaShop.Infastructure.Entities
{
    public class UserAddress : BaseEntity
    {
        [Required]
        [StringLength(500)]
        public string? FullName { get; set; }
        [StringLength(15)]
        public string? PhoneNumber { get; set; }
        [StringLength(1000)]
        public string? Address { get; set; }
        [StringLength(500)]
        public string? Email { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public string? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }
    }
}
