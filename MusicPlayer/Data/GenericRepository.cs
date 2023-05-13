using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace MusicPlayer.Data;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class, IBaseEntity
{
    private readonly DbSet<TEntity> _dbSet;

    public GenericRepository(DbContext dbContext)
    {
        _dbSet = dbContext.Set<TEntity>();
    }

    public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>>? expression)
    {
        return _dbSet.Where(expression).AsQueryable();
    }

    public void Add(TEntity entity)
    {
        _dbSet.Add(entity);
    }

    public void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public IQueryable<TEntity> GetAll()
    {
        return _dbSet.AsQueryable();
    }

    public async Task<TEntity?> FirstOrDefaultAsync()
    {
        return await _dbSet.FirstOrDefaultAsync();
    }

    public TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> expression)
    {
        return _dbSet.FirstOrDefault(expression);
    }

    public void AddRange(IEnumerable<TEntity> entities)
    {
        _dbSet.AddRange(entities);
    }

    public void DeleteRange(IEnumerable<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public bool Any(Expression<Func<TEntity, bool>>? expression = null) => expression != null && _dbSet.Any(expression);
    
    public IIncludableQueryable<TEntity, TProperty> Include<TProperty>(Expression<Func<TEntity, TProperty>> expression)
    {
        return _dbSet.Include(expression);
    }
}