using Microsoft.EntityFrameworkCore;
using RO.DevTest.Domain.Common.Models;
using RO.DevTest.Domain.Common.Parameters;
using RO.DevTest.Domain.Contracts.Persistance.Repositories;
using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System;

namespace RO.DevTest.Persistence.Repositories;

public class BaseRepository<T>(DefaultContext defaultContext) : IBaseRepository<T> where T : class
{
    private readonly DefaultContext _defaultContext = defaultContext;

    protected DefaultContext Context { get => _defaultContext; }

    public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await Context.Set<T>().AddAsync(entity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task Update(T entity)
    {
        Context.Set<T>().Update(entity);
        await Context.SaveChangesAsync();
    }

    public async Task Delete(T entity)
    {
        Context.Set<T>().Remove(entity);
        await Context.SaveChangesAsync();
    }

    public T? Get(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        => GetQueryWithIncludes(predicate, includes).FirstOrDefault();

    public async Task<PagedList<T>> GetPagedAsync(
        PaginationParameters parameters,
        Expression<Func<T, bool>>? predicate = null,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = Context.Set<T>().AsNoTracking();

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        if (!string.IsNullOrWhiteSpace(parameters.Filter))
        {
            try
            {
                query = query.Where(parameters.Filter);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error applying filter '{parameters.Filter}': {ex.Message}");
            }
        }

        if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
        {
            try
            {
                query = query.OrderBy(parameters.OrderBy);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error applying order by '{parameters.OrderBy}': {ex.Message}");
                try { query = query.OrderBy("Id"); } catch { /* Ignore if default sort fails */ }
            }
        }
        else
        {
             try { query = query.OrderBy("Id"); } catch { /* Ignore if default sort fails */ }
        }

        return await PagedList<T>.CreateAsync(query, parameters.PageNumber, parameters.PageSize);
    }

    private IQueryable<T> GetQueryWithIncludes(
        Expression<Func<T, bool>> predicate,
        params Expression<Func<T, object>>[] includes
    )
    {
        IQueryable<T> baseQuery = GetWhereQuery(predicate);

        foreach (Expression<Func<T, object>> include in includes)
        {
            baseQuery = baseQuery.Include(include);
        }

        return baseQuery;
    }

    private IQueryable<T> GetWhereQuery(Expression<Func<T, bool>> predicate)
    {
        IQueryable<T> baseQuery = Context.Set<T>();

        if (predicate is not null)
        {
            baseQuery = baseQuery.Where(predicate);
        }

        return baseQuery;
    }
}
