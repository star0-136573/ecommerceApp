using ecommerceApp.Models;

namespace ecommerceApp.Repository.IRepository
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {

		
		void Update(OrderDetail obj);
        void Save();
    }
}
