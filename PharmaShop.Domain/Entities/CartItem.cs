﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmaShop.Domain.Entities
{
    public class CartItem : BaseEntity
    {
        [Required]
        public int CartId { get; set; }

        [ForeignKey(nameof(CartId))]
        public Cart? Cart { get; set; }
        [Required]
        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }

        public int Quantity { get; set; }
    }
}
