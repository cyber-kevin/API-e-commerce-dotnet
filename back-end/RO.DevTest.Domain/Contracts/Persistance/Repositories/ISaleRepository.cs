using RO.DevTest.Domain.Common.Models;
using RO.DevTest.Domain.Entities;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RO.DevTest.Domain.Contracts.Persistance.Repositories;

public interface ISaleRepository : IBaseRepository<Sale>
{
    Task<SalesAnalysisResult> GetSalesAnalysisAsync(DateTime startDate, DateTime endDate);
    
    Task<Sale?> GetByIdAsync(Guid id, Expression<Func<Sale, object>>[]? includes = null);
    Task<bool> DeleteByIdAsync(Guid id);
    Task<bool> AddAsync(Sale entity);
    Task<bool> UpdateAsync(Sale entity);
}
