using System;

namespace ShelfSimple.Api.Services
{
    public class ProductNotFoundException : Exception
    {
        public ProductNotFoundException(int productId)
            : base("Product " + productId + " does not exist")
        {
            ProductId = productId;
        }

        public int ProductId { get; }
    }
}
