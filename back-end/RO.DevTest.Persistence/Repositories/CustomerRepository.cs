using Microsoft.EntityFrameworkCore;

using RO.DevTest.Persistence.Repositories;
using RO.DevTest.Domain.Entities;

namespace RO.DevTest.Persistence.Repositories
{
    public class CustomerRepository : BaseRepository<Customer>
    {
    private readonly DefaultContext _context;

    public CustomerRepository(DefaultContext context) : base(context)
    {
        _context = context;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await _context.Set<Customer>().ToListAsync();
    }

    public async Task AddAsync(Customer customer)
    {
        await _context.Set<Customer>().AddAsync(customer);
    }

    public async Task<bool> UpdateAsync(Customer customer)
    {
        var existingCustomer = await _context.Set<Customer>().FindAsync(customer.Id);
        if (existingCustomer == null)
        {
            return false;
        }

        _context.Entry(existingCustomer).CurrentValues.SetValues(customer);
        existingCustomer.ModifiedOn = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteByIdAsync(Guid id)
    {
        var customer = await _context.Set<Customer>().FindAsync(id);
        if (customer == null)
        {
            return false;
        }

        _context.Set<Customer>().Remove(customer);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Customer?> GetByIdAsync(Guid id)
    {
        return await _context.Set<Customer>().FindAsync(id);
    }
    }
}
