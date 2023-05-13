using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
using MusicPlayer.Data;

namespace MusicPlayer.Models;

/// <summary>
/// Альбом
/// </summary>
public class Album : BaseEntity
{
    /// <summary>
    /// Идентификатор исполнителя
    /// </summary>
    public long? ArtistId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Наименование
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Название файла обложки
    /// </summary>
    public string? AlbumArtFileName { get; set; }

    /// <summary>
    /// Исполнитель
    /// </summary>
    public virtual Artist Artist { get; set; }

    /// <summary>
    /// Композиции
    /// </summary>
    public virtual IList<Track> Tracks { get; set; }
    
    /// <summary>
    /// Пользователь 
    /// </summary>
    public virtual IdentityUser User { get; set; }
}