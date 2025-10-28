using System.Linq.Expressions;

namespace om.servicing.casemanagement.data.Repositories.Shared;

public interface IGenericRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Asynchronously retrieves an entity by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to retrieve. Must not be null.</param>
    /// <param name="includePaths">An optional array of navigation property paths to include in the query (dot-separated for nested includes).  If null, no related entities are
    /// included.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the entity of type <typeparamref
    /// name="TEntity"/>  if found; otherwise, <see langword="null"/>.</returns>
    Task<TEntity?> GetByIdAsync(object id, string[]? includePaths = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(string[]? includePaths = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, string[]? includePaths = null, CancellationToken cancellationToken = default);

    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);
}
