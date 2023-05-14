using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicPlayer.Data;
using MusicPlayer.Models;
using MusicPlayer.Services.Interfaces;

namespace MusicPlayer.Services.Implementation;

public class ArtistService : IArtistService
{
    private readonly IUnitOfWork _unitOfWork;

    public ArtistService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Artist[]> GetArtists(IdentityUser user)
    {
        var artists = await _unitOfWork.GetRepository<Artist>()
            .Where(x => x.UserId == user.Id)
            .ToArrayAsync();

        return artists;
    }

    public async Task<Artist> GetArtistById(long artistId, IdentityUser user)
    {
        var artist = _unitOfWork.GetRepository<Artist>()
            .FirstOrDefault(x => x.UserId == user.Id && x.Id == artistId);
        
        artist.Tracks = await _unitOfWork.GetRepository<Track>()
            .Where(x => x.ArtistId == artistId && x.UserId == user.Id)
            .Include(x => x.Album)
            .Include(x => x.Artist)
            .ToListAsync();

        return artist;
    }
}