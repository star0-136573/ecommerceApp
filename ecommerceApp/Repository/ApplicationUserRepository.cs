using ecommerceApp.Data;
using ecommerceApp.Models;
using ecommerceApp.Repository.IRepository;

namespace ecommerceApp.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private AppdbContext _db;
        public ApplicationUserRepository(AppdbContext db) : base(db)
        {
            _db = db;
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
