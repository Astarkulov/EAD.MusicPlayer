namespace MusicPlayer.Dto;

public class PlaylistData
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
    /// Отмеченный плейлист
    /// </summary>
    public bool CheckedPlaylist { get; set; }
}