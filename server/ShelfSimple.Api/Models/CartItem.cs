using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShelfSimple.Api.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        [Index("IX_CartItem_CartProduct", 1, IsUnique = true)]
        public Guid CartId { get; set; }
        public virtual Cart Cart { get; set; }

        [Index("IX_CartItem_CartProduct", 2, IsUnique = true)]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public int Quantity { get; set; }
    }
}
