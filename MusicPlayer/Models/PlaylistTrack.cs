using System.ComponentModel.DataAnnotations;
using MusicPlayer.Data;

namespace MusicPlayer.Models;

public class PlaylistTrack : IBaseEntity
{
    /// <summary>
    /// Идентификатор плейлиста
    /// </summary>
    public long? PlaylistId { get; set; }

    /// <summary>
    /// Идентификатор трека
    /// </summary>
    public long? TrackId { get; set; }

    public virtual Playlist Playlist { get; set; }

    public virtual Track Track { get; set; }
}