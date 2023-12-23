using MusicPlayer.Models;

namespace MusicPlayer.Services.Interfaces;

public interface IAlbumService
{
    Task<Album[]> GetAlbums();
    Task<Album> GetAlbumById(long albumId);
}