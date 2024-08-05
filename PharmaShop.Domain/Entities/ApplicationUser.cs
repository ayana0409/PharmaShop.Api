using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmaShop.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(500)]
        public string? FullName { get; set; }
        [StringLength(1000)]
        public string? Address { get; set; }
        public string? MobilePhone { get; set; }
        public int Point { get; set; }
        [Required]
        public bool IsActive { get; set; }

        public int? TypeId { get; set; }
        [ForeignKey(nameof(TypeId))]
        public UserType? Type { get; set; }
    }
}
