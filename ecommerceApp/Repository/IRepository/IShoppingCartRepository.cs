using ecommerceApp.Models;

namespace ecommerceApp.Repository.IRepository
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {


        void Update(ShoppingCart obj);
        void Save();
       
    }
}
