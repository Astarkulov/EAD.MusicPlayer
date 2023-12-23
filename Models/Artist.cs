using Microsoft.AspNetCore.Identity;
using MusicPlayer.Data;

namespace MusicPlayer.Models;

/// <summary>
/// Исполнитель
/// </summary>
public class Artist : BaseEntity
{
    /// <summary>
    /// ФИО
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Название файла фотографии
    /// </summary>
    public string? ArtistArtFileName { get; set; }

    /// <summary>
    /// Композиции
    /// </summary>
    public virtual IList<Track> Tracks { get; set; }

    /// <summary>
    /// Альбомы
    /// </summary>
    public virtual IList<Album> Albums { get; set; }
}