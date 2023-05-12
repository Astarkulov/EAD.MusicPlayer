using Microsoft.AspNetCore.Identity;
using MusicPlayer.Data;

namespace MusicPlayer.Models;

/// <summary>
/// Исполнитель
/// </summary>
public class Artist : BaseEntity
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public string UserId { get; set; }
    
    /// <summary>
    /// ФИО
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Композиции
    /// </summary>
    public virtual IList<Track> Tracks { get; set; }

    /// <summary>
    /// Альбомы
    /// </summary>
    public virtual IList<Album> Albums { get; set; }
    
    /// <summary>
    /// Пользователь
    /// </summary>
    public IdentityUser User { get; set; }
}