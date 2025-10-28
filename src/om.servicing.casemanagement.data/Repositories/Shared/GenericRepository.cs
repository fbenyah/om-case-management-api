using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace om.servicing.casemanagement.data.Repositories.Shared;

/// <summary>
/// Provides a generic implementation of a repository pattern for managing entities in a database.
/// var repo = new GenericRepository<OMCase, CaseManagerContext>(context);
/// </summary>
/// <remarks>This class offers common data access operations such as retrieving, adding, updating, and removing
/// entities. It is designed to work with Entity Framework Core and assumes that the provided <typeparamref
/// name="TContext"/>  is properly configured to manage the <typeparamref name="TEntity"/> type.</remarks>
/// <typeparam name="TEntity">The type of the entity managed by the repository. Must be a reference type.</typeparam>
/// <typeparam name="TContext">The type of the database context used by the repository. Must derive from <see cref="DbContext"/>.</typeparam>
public class GenericRepository<TEntity, TContext> : IGenericRepository<TEntity>
    where TEntity : class
    where TContext : DbContext
{
    protected readonly TContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public GenericRepository(TContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }

    /// <summary>
    /// Asynchronously retrieves an entity by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to retrieve. Must not be null.</param>
    /// <param name="includePaths">An optional array of navigation property paths to include in the query (dot-separated for nested includes).  If null, no related entities are
    /// included.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the entity of type <typeparamref
    /// name="TEntity"/>  if found; otherwise, <see langword="null"/>.</returns>
    public async Task<TEntity?> GetByIdAsync(object id, string[]? includePaths = null, CancellationToken cancellationToken = default)
    {
        // If no includes requested, use FindAsync (uses primary key lookup and is efficient)
        if (includePaths == null || includePaths.Length == 0)
        {
            return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }

        // Build query with includes
        IQueryable<TEntity> query = IncludePaths(includePaths);

        // Resolve primary key metadata to build a predicate
        var entityType = _context.Model.FindEntityType(typeof(TEntity));
        var primaryKey = entityType?.FindPrimaryKey();
        if (primaryKey == null || primaryKey.Properties.Count == 0)
            throw new InvalidOperationException($"Entity {typeof(TEntity).Name} does not have a primary key defined.");

        // Single key common case
        if (primaryKey.Properties.Count == 1)
        {
            var keyProp = primaryKey.Properties[0];
            var propName = keyProp.Name;

            var parameter = Expression.Parameter(typeof(TEntity), "e");
            var left = Expression.PropertyOrField(parameter, propName);

            // convert incoming id to the property CLR type
            var converted = Convert.ChangeType(id, left.Type);
            var constant = Expression.Constant(converted, left.Type);

            var body = Expression.Equal(left, constant);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameter);

            return await query.FirstOrDefaultAsync(lambda, cancellationToken);
        }

        // Composite key: expect id to be object[] with values in key order
        if (id is not object[] keyValues || keyValues.Length != primaryKey.Properties.Count)
            throw new ArgumentException("For composite primary keys, provide id as object[] with values in key order.", nameof(id));

        var param = Expression.Parameter(typeof(TEntity), "e");
        Expression? composite = null;
        for (int i = 0; i < primaryKey.Properties.Count; i++)
        {
            var prop = primaryKey.Properties[i];
            var left = Expression.PropertyOrField(param, prop.Name);
            var converted = Convert.ChangeType(keyValues[i], left.Type);
            var constant = Expression.Constant(converted, left.Type);
            var equal = Expression.Equal(left, constant);
            composite = composite == null ? equal : Expression.AndAlso(composite, equal);
        }

        var compositeLambda = Expression.Lambda<Func<TEntity, bool>>(composite!, param);
        return await query.FirstOrDefaultAsync(compositeLambda, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(string[]? includePaths = null, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = IncludePaths(includePaths);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, string[]? includePaths = null, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = IncludePaths(includePaths);

        return await query.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<TEntity> IncludePaths(string[]? includePaths = null)
    {
        IQueryable<TEntity> query = _dbSet;

        if (includePaths != null)
        {
            foreach (var path in includePaths)
            {
                if (!string.IsNullOrWhiteSpace(path))
                {
                    query = query.Include(path);
                }
            }
        }

        return query;
    }
}
