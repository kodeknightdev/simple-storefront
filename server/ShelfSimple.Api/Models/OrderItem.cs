using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShelfSimple.Api.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        // copied at checkout time so later edits to the product don't change old orders
        [Index]
        public int SellerId { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; }

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }
    }
}
