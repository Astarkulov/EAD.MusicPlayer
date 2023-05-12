using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using MusicPlayer.Data;

namespace MusicPlayer.Models;

/// <summary>
/// Композиция
/// </summary>
public class Track : BaseEntity
{
    /// <summary>
    /// Идентификатор плейлиста
    /// </summary>
    public long? PlaylistId { get; set; }
    
    /// <summary>
    /// Идентификатор альбома
    /// </summary>
    public long? AlbumId { get; set; }

    /// <summary>
    /// Идентификатор исполнителя
    /// </summary>
    public long? ArtistId { get; set; }
    
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Наименование
    /// </summary>
    [MaxLength(300)]
    public string Name { get; set; }

    /// <summary>
    /// Альбом
    /// </summary>
    public virtual Album Album { get; set; }

    /// <summary>
    /// Исполнитель
    /// </summary>
    public virtual Artist Artist { get; set; }

    /// <summary>
    /// Плейлист
    /// </summary>
    public virtual Playlist Playlist { get; set; }
    
    /// <summary>
    /// Пользователь
    /// </summary>
    public IdentityUser User { get; set; }
}