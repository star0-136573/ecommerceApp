using ecommerceApp.Data;
using ecommerceApp.Models;
using ecommerceApp.Repository.IRepository;

namespace ecommerceApp.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private AppdbContext _db;
        public ShoppingCartRepository(AppdbContext db) : base(db)
        {
            _db = db;
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(ShoppingCart obj)
        {
            _db.ShoppingCarts.Update(obj);
        }
    }
}
