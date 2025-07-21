using Microsoft.EntityFrameworkCore;
using TastyGo.Data;
using TastyGo.Interfaces.IRepositories;
using TastyGo.Models;

namespace TastyGo.Repositories
{
    public class VendorRepository : IVendorRepository
    {
        private readonly TastyGoDbContext _dbContext;

        public VendorRepository(TastyGoDbContext dbContext)
        {
            _dbContext = dbContext;

        }

        public async Task<Vendor?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Vendors.Include(v => v.User)
                                         .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Vendor?> GetByUserIdAsync(Guid userId)
        {
            return await _dbContext.Vendors.FirstOrDefaultAsync(v => v.UserId == userId);
        }

        public void Add(Vendor vendor)
        {
            _dbContext.Vendors.Add(vendor);
        }

        public void MarkAsModified(Vendor vendor)
        {
            _dbContext.Entry(vendor).State = EntityState.Modified;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public void Delete(Vendor vendor)
        {
            _dbContext.Vendors.Remove(vendor);
        }
    }
}
