using MusicPlayer.Models;

namespace MusicPlayer.ViewModels;

public class PlaylistViewModel
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public long? Id { get; set; }
    
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
    public virtual Track[] Tracks { get; set; }
}