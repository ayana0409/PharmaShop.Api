using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmaShop.Domain.Entities
{
    public class Category : BaseEntity
    {
        [Required]
        [StringLength(500)]
        public string Name { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        [ForeignKey(nameof(ParentId))]
        public Category? ParentCategory { get; set; }
        public ICollection<Category>? Categories { get; set; }
        [Required]
        public bool IsAcvive { get; set; } = true;

    }
}
