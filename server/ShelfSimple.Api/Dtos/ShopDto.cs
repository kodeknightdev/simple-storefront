using System.Collections.Generic;

namespace ShelfSimple.Api.Dtos
{
    public class ShopDto
    {
        public string Slug { get; set; }
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public IList<ProductDto> Products { get; set; }
    }
}
