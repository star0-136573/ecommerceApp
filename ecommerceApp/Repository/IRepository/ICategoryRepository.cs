using ecommerceApp.Models;

namespace ecommerceApp.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
   
            void Update(Category obj);
            void Save();
        
    }
}
