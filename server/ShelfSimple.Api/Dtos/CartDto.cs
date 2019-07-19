using System;
using System.Collections.Generic;

namespace ShelfSimple.Api.Dtos
{
    public class CartDto
    {
        public Guid Id { get; set; }
        public IList<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        public int ItemCount { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class CartItemDto
    {
        public int ProductId { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
    }
}
