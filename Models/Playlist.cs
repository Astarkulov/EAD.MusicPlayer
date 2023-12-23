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
    /// Название файла плейлиста
    /// </summary>
    public string PlaylistArtFileName { get; set; }

    /// <summary>
    /// Композиции
    /// </summary>
    public virtual IList<PlaylistTrack> PlaylistTracks { get; set; }

    /// <summary>
    /// Пользователь
    /// </summary>
    public IdentityUser User { get; set; }
}