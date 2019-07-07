using System;
using System.ComponentModel.DataAnnotations;

namespace ShelfSimple.Api.Models
{
    public class Product
    {
        public int Id { get; set; }

        public int SellerId { get; set; }
        public virtual Seller Seller { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        // how many are in stock
        public int Quantity { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
