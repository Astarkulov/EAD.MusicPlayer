using Microsoft.AspNetCore.Identity;
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
    
    public async Task<Album[]> GetAlbums(IdentityUser user)
    {
        var albums = await _unitOfWork.GetRepository<Album>()
            .Where(x => x.UserId == user.Id)
            .ToArrayAsync();

        return albums;
    }

    public async Task<Album> GetAlbumById(long albumId, IdentityUser user)
    {
        var album = _unitOfWork.GetRepository<Album>()
            .FirstOrDefault(x => x.UserId == user.Id && x.Id == albumId);
        
        album.Tracks = await _unitOfWork.GetRepository<Track>()
            .Where(x => x.AlbumId == albumId && x.UserId == user.Id)
            .Include(x => x.Artist)
            .Include(x => x.Artist)
            .ToListAsync();

        return album;
    }
}