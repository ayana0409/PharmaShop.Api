﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PharmaShop.Domain.Entities
{
    public class Product : BaseEntity
    {
        [Required]
        [StringLength(500)]
        public string Name { get; set; } = string.Empty;
        [StringLength(2000)]
        public string? Description { get; set; }
        [Required]
        public double Taxing { get; set; }
        [Required]
        [StringLength(300)]
        public string Unit { get; set; } = string.Empty;
        [Required]
        [StringLength(500)]
        public string Packaging { get; set; } = string.Empty;
        [Required]
        public string Brand { get; set; } = string.Empty;
        public ICollection<Image>? Images { get; set; }
        public ICollection<ProductDetail>? Details { get; set; }
        public ICollection<ProductInventory>? ProductInventorys { get; set; }
        [Required]
        public int BigUnit { get; set; }
        public int MediumUnit { get; set; }
        public int SmallUnit { get; set; }
        [Required]
        public double BigUnitPrice { get; set; }
        public double MediumUnitPrice { get; set; }
        public double SmallUnitPrice { get; set; }
        [Required]
        public bool RequirePrescription { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }
    }
}
