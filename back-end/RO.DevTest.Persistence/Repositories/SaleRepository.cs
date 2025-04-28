using Microsoft.EntityFrameworkCore;
using RO.DevTest.Domain.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Persistence;
using RO.DevTest.Domain.Common.Models; 
using System.Linq.Expressions;

namespace RO.DevTest.Persistence.Repositories;

public class SaleRepository : BaseRepository<Sale>, ISaleRepository
{
    public SaleRepository(DefaultContext dbContext) : base(dbContext) 
    {
    }

    public async Task<SalesAnalysisResult> GetSalesAnalysisAsync(DateTime startDate, DateTime endDate)
    {
        startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
        endDate = DateTime.SpecifyKind(endDate.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

        var salesInPeriod = await Context.Sales
            .Include(s => s.Items)
                .ThenInclude(i => i.Product) 
            .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
            .ToListAsync();

        var totalSalesCount = salesInPeriod.Count;
        var totalRevenue = salesInPeriod.Sum(s => s.Items.Sum(i => i.TotalValue));

        var productRevenues = salesInPeriod
            .SelectMany(s => s.Items)
            .GroupBy(i => new { i.ProductId, i.Product.Name })
            .Select(g => new ProductRevenueResult 
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.Name,
                TotalRevenue = g.Sum(i => i.TotalValue)
            })
            .ToList();

        return new SalesAnalysisResult 
        {
            TotalSalesCount = totalSalesCount,
            TotalRevenue = totalRevenue,
            ProductRevenues = productRevenues
        };
    }

    public async Task<Sale?> GetByIdAsync(Guid id, Expression<Func<Sale, object>>[]? includes = null)
    {
        IQueryable<Sale> query = Context.Set<Sale>();

        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        return await query.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<bool> DeleteByIdAsync(Guid id)
    {
        var entity = await Context.Set<Sale>().FindAsync(id);
        if (entity == null)
            return false;

        Context.Set<Sale>().Remove(entity);
        await Context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddAsync(Sale entity)
    {
        try
        {
            await Context.Set<Sale>().AddAsync(entity);
            await Context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateAsync(Sale entity)
    {
        try
        {
            Context.Set<Sale>().Update(entity);
            await Context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
