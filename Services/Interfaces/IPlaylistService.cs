using Microsoft.AspNetCore.Identity;
using MusicPlayer.Dto;
using MusicPlayer.Models;
using MusicPlayer.ViewModels;

namespace MusicPlayer.Services.Interfaces;

public interface IPlaylistService
{
    Task<Playlist[]> GetAllPlaylists(IdentityUser user);
    Playlist GetPlaylistById(long? playlistId);
    Task SavePlaylist(string playlistName, IFormFile imageFile, long? playlistId, IdentityUser user);
    Task<PlaylistViewModel> GetPlaylistTracks(long? playlistId);
    Task<PlaylistData[]> GetPlaylistsForModal(long? trackId, IdentityUser user);
    Task DeletePlaylist(long playlistId);
    Task DeletePlaylistFromPlaylist(long? trackId, long playlistId);
}