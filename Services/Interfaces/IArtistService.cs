using MusicPlayer.Models;

namespace MusicPlayer.Services.Interfaces;

public interface IArtistService
{
    Task<Artist[]> GetArtists();
    Task<Artist> GetArtistById(long artistId);
}