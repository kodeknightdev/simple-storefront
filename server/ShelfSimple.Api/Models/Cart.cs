using System;
using System.Collections.Generic;

namespace ShelfSimple.Api.Models
{
    // Anonymous cart - the id doubles as the token the browser keeps around.
    public class Cart
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
