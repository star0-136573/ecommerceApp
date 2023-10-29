using ecommerceApp.Data;
using ecommerceApp.Models;
using ecommerceApp.Repository.IRepository;

namespace ecommerceApp.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private AppdbContext _db;
        public OrderDetailRepository(AppdbContext db) : base(db)
        {
            _db = db;
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(OrderDetail obj)
        {
            _db.Update(obj);
        }

	
	}
}
