using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShelfSimple.Api.Models
{
    public class Seller
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string DisplayName { get; set; }

        [Required, MaxLength(256)]
        [Index(IsUnique = true)]
        public string Email { get; set; }

        // null for the seeded dev sellers until they set one
        [MaxLength(200)]
        public string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
