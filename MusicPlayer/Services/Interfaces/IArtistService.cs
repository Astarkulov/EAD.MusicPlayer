using Microsoft.AspNetCore.Identity;
using MusicPlayer.Models;

namespace MusicPlayer.Services.Interfaces;

public interface IArtistService
{
    Task<Artist[]> GetArtists(IdentityUser user);
    Task<Artist> GetArtistById(long artistId, IdentityUser user);
}