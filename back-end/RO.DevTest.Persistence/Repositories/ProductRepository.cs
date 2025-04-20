using Microsoft.EntityFrameworkCore;

using RO.DevTest.Persistence.Repositories;
using RO.DevTest.Domain.Entities;

namespace RO.DevTest.Persistence.Repositories

{
    public class ProductRepository : BaseRepository<Product>
    {
        private readonly DefaultContext _context;

        public ProductRepository(DefaultContext context) : base(context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    
        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Set<Product>().FindAsync(id);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Set<Product>().ToListAsync();
        }

        public async Task AddAsync(Product product)
        {
            await _context.Set<Product>().AddAsync(product);
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            var existingProduct = await _context.Products.FindAsync(product.Id);
            if (existingProduct == null)
                return false;

            _context.Entry(existingProduct).CurrentValues.SetValues(product);
            existingProduct.ModifiedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteByIdAsync(Guid id)
        {
            var entity = await _context.Products.FindAsync(id);
            if (entity == null)
                return false;

            _context.Products.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
