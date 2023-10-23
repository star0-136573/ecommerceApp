using ecommerceApp.Data;
using ecommerceApp.Models;
using ecommerceApp.Repository.IRepository;

namespace ecommerceApp.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private AppdbContext _db;
        public CategoryRepository(AppdbContext db) : base(db)
        {

            _db = db;
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(Category obj)
        {
            _db.Categories.Update(obj);
        }
    }
}
