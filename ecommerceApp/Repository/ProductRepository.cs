using ecommerceApp.Data;
using ecommerceApp.Models;
using ecommerceApp.Repository.IRepository;

namespace ecommerceApp.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private AppdbContext _db;
        public ProductRepository(AppdbContext db) : base(db)
        {

            _db = db;
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(Product obj)
        {
            _db.Products.Update(obj);
        }

      
    }
}
