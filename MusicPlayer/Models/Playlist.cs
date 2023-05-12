using Microsoft.AspNetCore.Identity;
using MusicPlayer.Data;

namespace MusicPlayer.Models;

/// <summary>
/// Плейлист
/// </summary>
public class Playlist : BaseEntity
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public string UserId { get; set; }
    
    /// <summary>
    /// Наименование
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Композиции
    /// </summary>
    public virtual IList<Track> Tracks { get; set; }

    /// <summary>
    /// Пользователь
    /// </summary>
    public IdentityUser User { get; set; }
}