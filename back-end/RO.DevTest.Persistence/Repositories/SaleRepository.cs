using Microsoft.EntityFrameworkCore;
using RO.DevTest.Domain.Entities;

namespace RO.DevTest.Persistence.Repositories
{
    public class SaleRepository : BaseRepository<Sale>
    {
        private readonly DefaultContext _context;

        public SaleRepository(DefaultContext context) : base(context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Sale?> GetByIdAsync(Guid id)
        {
            return await _context.Sales
                .Include(s => s.Items)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Sale>> GetAllAsync()
        {
            return await _context.Sales
            .Include(s => s.Items)
            .ToListAsync();
        }

        public async Task AddAsync(Sale sale)
        {
            await _context.Set<Sale>().AddAsync(sale);
        }

        public async Task<bool> UpdateAsync(Sale sale)
        {
            var existingSale = await _context.Sales.FindAsync(sale.Id);
            if (existingSale == null)
                return false;

            _context.Entry(existingSale).CurrentValues.SetValues(sale);
            existingSale.ModifiedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteByIdAsync(Guid id)
        {
            var entity = await _context.Sales.FindAsync(id);
            if (entity == null)
                return false;

            _context.Sales.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
