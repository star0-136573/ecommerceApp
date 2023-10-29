namespace ecommerceApp.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }

        public OrderDetail OrderDetail { get; set; }
    }
}
