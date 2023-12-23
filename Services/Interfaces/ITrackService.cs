using Microsoft.AspNetCore.Identity;
using MusicPlayer.Models;

namespace MusicPlayer.Services.Interfaces;

public interface ITrackService
{
    Task DeleteTrack(long trackId);
    Task<string> AddTrack(IFormFile file);
    Task AddTrackToPlaylist(IEnumerable<long> playlistIds, long trackId);
    Task<Track[]> GetAllTracks();
    Task<Track[]> GetFilteredTracks(string searchText);
    Task<bool> IsAdminUser(IdentityUser user);
}