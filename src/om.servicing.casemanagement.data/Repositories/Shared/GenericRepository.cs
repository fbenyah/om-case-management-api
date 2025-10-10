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

    public async Task<TEntity?> GetByIdAsync(object id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(TEntity entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
