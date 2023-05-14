using Microsoft.AspNetCore.Identity;
using MusicPlayer.Models;

namespace MusicPlayer.Services.Interfaces;

public interface IAlbumService
{
    Task<Album[]> GetAlbums(IdentityUser user);
    Task<Album> GetAlbumById(long albumId, IdentityUser user);
}