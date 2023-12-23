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

    public async Task<Artist[]> GetArtists()
    {
        var artists = await _unitOfWork.GetRepository<Artist>()
            .GetAll()
            .ToArrayAsync();

        return artists;
    }

    public async Task<Artist> GetArtistById(long artistId)
    {
        var artist = _unitOfWork.GetRepository<Artist>()
            .FirstOrDefault(x => x.Id == artistId);

        artist.Tracks = await _unitOfWork.GetRepository<Track>()
            .Where(x => x.ArtistId == artistId)
            .Include(x => x.Album)
            .Include(x => x.Artist)
            .ToListAsync();

        return artist;
    }
}