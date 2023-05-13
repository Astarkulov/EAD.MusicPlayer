using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicPlayer.Data;
using MusicPlayer.Models;

namespace MusicPlayer.Controllers;

public class AlbumController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;

    public AlbumController(UserManager<IdentityUser> userManager,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var albums = await _unitOfWork.GetRepository<Album>()
            .Where(x => x.UserId == user.Id)
            .ToArrayAsync();

        return View(albums);
    }

    public async Task<IActionResult> OpenTracks(long albumId)
    {
        var user = await _userManager.GetUserAsync(User);
        var album = _unitOfWork.GetRepository<Album>()
            .FirstOrDefault(x => x.UserId == user.Id && x.Id == albumId);
        if (album is null) return View();
        
        album.Tracks = await _unitOfWork.GetRepository<Track>()
            .Where(x => x.AlbumId == albumId && x.UserId == user.Id)
            .Include(x => x.Artist)
            .Include(x => x.Playlist)
            .ToListAsync();
        ViewBag.AlbumArtFileName = album.AlbumArtFileName;

        return View(album.Tracks.ToArray());
    }
}