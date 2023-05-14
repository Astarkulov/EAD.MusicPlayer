using Microsoft.AspNetCore.Identity;
using MusicPlayer.Models;

namespace MusicPlayer.Services.Interfaces;

public interface ITrackService
{
    Task DeleteTrack(long trackId);
    Task<string> AddTrack(IFormFile file, IdentityUser user);
    Task AddTrackToPlaylist(IEnumerable<long> playlistIds, long trackId);
    Task<Track[]> GetAllTracks(IdentityUser user);
    Task<Track[]> GetFilteredTracks(IdentityUser user, string searchText);
}