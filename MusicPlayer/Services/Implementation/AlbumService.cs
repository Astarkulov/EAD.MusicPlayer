using Microsoft.EntityFrameworkCore;
using MusicPlayer.Data;
using MusicPlayer.Models;
using MusicPlayer.Services.Interfaces;

namespace MusicPlayer.Services.Implementation;

public class AlbumService : IAlbumService
{
    private readonly IUnitOfWork _unitOfWork;

    public AlbumService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Album[]> GetAlbums()
    {
        var albums = await _unitOfWork.GetRepository<Album>()
            .GetAll()
            .ToArrayAsync();

        return albums;
    }

    public async Task<Album> GetAlbumById(long albumId)
    {
        var album = _unitOfWork.GetRepository<Album>()
            .FirstOrDefault(x => x.Id == albumId);

        album.Tracks = await _unitOfWork.GetRepository<Track>()
            .Where(x => x.AlbumId == albumId)
            .Include(x => x.Artist)
            .Include(x => x.Artist)
            .ToListAsync();

        return album;
    }
}