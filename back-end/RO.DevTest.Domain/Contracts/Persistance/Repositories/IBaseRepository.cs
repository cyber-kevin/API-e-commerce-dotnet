using RO.DevTest.Domain.Common.Models;
using RO.DevTest.Domain.Common.Parameters;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RO.DevTest.Domain.Contracts.Persistance.Repositories;

public interface IBaseRepository<T> where T : class {

    /// <summary>
    /// Creates a new entity in the database
    /// </summary>
    /// <param name="entity"> The entity to be create </param>
    /// <param name="cancellationToken"> Cancellation token </param>
    /// <returns> The created entity </returns>
    Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds the first entity that matches with the <paramref name="predicate"/>
    /// </summary>
    /// <param name="predicate">
    /// The <see cref="Expression"/> to be used while
    /// looking for the entity
    /// </param>
    /// <returns>
    /// The <typeparamref name="T"/> entity, if found. Null otherwise. </returns>
    T? Get(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Gets a paged list of entities based on pagination, filtering, and sorting parameters.
    /// </summary>
    /// <param name="parameters">Pagination, filtering, and sorting parameters.</param>
    /// <param name="predicate">Optional filter predicate.</param>
    /// <param name="includes">Optional navigation properties to include.</param>
    /// <returns>A paged list of entities.</returns>
    Task<PagedList<T>> GetPagedAsync(
        PaginationParameters parameters,
        Expression<Func<T, bool>>? predicate = null,
        params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Updates an entity entry on the database
    /// </summary>
    /// <param name="entity"> The entity to be added </param>
    /// <returns>A task representing the asynchronous operation.</returns> // Updated documentation
    Task Update(T entity); // Changed return type to Task

    /// <summary>
    /// Deletes one entry from the database
    /// </summary>
    /// <param name="entity"> The entity to be deleted </param>
    /// <returns>A task representing the asynchronous operation.</returns> // Updated documentation
    Task Delete(T entity); // Changed return type to Task
}
