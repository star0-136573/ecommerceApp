using ecommerceApp.Models;

namespace ecommerceApp.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {

        void Update(Product obj);
        void Save();

    }
}