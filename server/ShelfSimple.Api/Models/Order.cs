using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShelfSimple.Api.Models
{
    public enum OrderStatus
    {
        Pending = 0,
        Paid = 1,
        Failed = 2
    }

    public class Order
    {
        public int Id { get; set; }

        public Guid CartId { get; set; }

        [Required, MaxLength(256)]
        public string Email { get; set; }

        public OrderStatus Status { get; set; }

        public decimal Total { get; set; }

        [Required, MaxLength(3)]
        public string Currency { get; set; }

        [MaxLength(100)]
        [Index]
        public string PaymentIntentId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }

        public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
