using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace MusicPlayer.Data;

public interface IGenericRepository<TEntity> where TEntity : class, IBaseEntity
{
    IQueryable<TEntity> Where(Expression<Func<TEntity, bool>>? expression = null);

    void Add(TEntity entity);

    void Update(TEntity entity);

    void Delete(TEntity entity);

    IQueryable<TEntity> GetAll();

    Task<TEntity?> FirstOrDefaultAsync();

    TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> expression);

    void AddRange(IEnumerable<TEntity> entities);

    void DeleteRange(IEnumerable<TEntity> entities);

    Task AddAsync(TEntity entity);

    Task AddRangeAsync(IEnumerable<TEntity> entities);

    bool Any(Expression<Func<TEntity, bool>>? expression = null);

    IIncludableQueryable<TEntity, TProperty> Include<TProperty>(Expression<Func<TEntity, TProperty>> expression);
}