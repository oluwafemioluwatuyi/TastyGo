using TastyGo.Models;

namespace TastyGo.Interfaces.IRepositories
{
    public interface IVendorRepository
    {
        Task<Vendor?> GetByIdAsync(Guid id);
        Task<Vendor?> GetByUserIdAsync(Guid userId);
        void Add(Vendor vendor);
        void Delete(Vendor vendor);
        void MarkAsModified(Vendor vendor);
        Task<bool> SaveChangesAsync();
    }
}
