using System.Collections.Concurrent;
using MusicPlayer.Models;

namespace MusicPlayer.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ConcurrentDictionary<Type, object> _repsDictionary;
    private readonly ApplicationDbContext _dbContext;

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _repsDictionary = new ConcurrentDictionary<Type, object>();
    }

    public IGenericRepository<TEntity> GetRepository<TEntity>()
        where TEntity : class, IBaseEntity
    {
        if (_repsDictionary.TryGetValue(typeof(TEntity), out object repository))
            return (IGenericRepository<TEntity>)repository;
        repository = new GenericRepository<TEntity>(_dbContext);
        _repsDictionary.TryAdd(typeof(TEntity), repository);
        return (IGenericRepository<TEntity>)repository;
    }

    public async Task SaveChanges()
    {
        await _dbContext.SaveChangesAsync();
    }

    private bool _disposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _dbContext.Dispose();
        }

        _disposed = true;
    }

    ~UnitOfWork() => Dispose(false);
}