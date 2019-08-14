using System.ComponentModel.DataAnnotations;

namespace ShelfSimple.Api.Dtos
{
    public class ProductInput
    {
        [Required, MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(4000)]
        public string Description { get; set; }

        [MaxLength(500)]
        public string ImageUrl { get; set; }

        [MaxLength(50)]
        public string Category { get; set; }

        [Range(0.5, 10000)]
        public decimal Price { get; set; }

        [Range(0, 9999)]
        public int Quantity { get; set; }
    }
}
