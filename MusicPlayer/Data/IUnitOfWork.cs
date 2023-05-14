using Microsoft.AspNetCore.Identity;
using MusicPlayer.Models;

namespace MusicPlayer.Data;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IBaseEntity;
    Task SaveChanges();
    Task<bool> IsAdminUser(IdentityUser user);
}