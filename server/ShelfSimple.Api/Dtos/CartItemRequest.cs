namespace ShelfSimple.Api.Dtos
{
    public class CartItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
