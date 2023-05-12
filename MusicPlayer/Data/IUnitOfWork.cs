using MusicPlayer.Models;

namespace MusicPlayer.Data;

public interface IUnitOfWork : IDisposable
{
    public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IBaseEntity;
    public Task SaveChanges();
}