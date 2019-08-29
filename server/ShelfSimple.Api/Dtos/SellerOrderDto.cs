using System;
using System.Collections.Generic;

namespace ShelfSimple.Api.Dtos
{
    // One order as a single seller sees it - only their own line items.
    public class SellerOrderDto
    {
        public int OrderId { get; set; }
        public string BuyerEmail { get; set; }
        public DateTime? PaidAt { get; set; }
        public decimal Total { get; set; }
        public IList<SellerOrderItemDto> Items { get; set; }
    }

    public class SellerOrderItemDto
    {
        public int ProductId { get; set; }
        public string Title { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
