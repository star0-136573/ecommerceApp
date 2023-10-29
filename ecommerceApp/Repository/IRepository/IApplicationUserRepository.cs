using ecommerceApp.Models;

namespace ecommerceApp.Repository.IRepository
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
        void Save();
    }
}
