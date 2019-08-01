using System;
using System.ComponentModel.DataAnnotations;

namespace ShelfSimple.Api.Dtos
{
    public class CheckoutRequest
    {
        public Guid CartId { get; set; }

        [Required, EmailAddress, MaxLength(256)]
        public string Email { get; set; }
    }

    public class CheckoutResponse
    {
        public int OrderId { get; set; }
        public string ClientSecret { get; set; }
        public decimal Total { get; set; }
        public string Currency { get; set; }
    }
}
