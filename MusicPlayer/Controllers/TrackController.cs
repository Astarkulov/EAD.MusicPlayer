using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicPlayer.Data;
using MusicPlayer.Models;
using MusicPlayer.ViewModels;

namespace MusicPlayer.Controllers;

public class TrackController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;

    public TrackController(UserManager<IdentityUser> userManager,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        var tracks = await _unitOfWork
            .GetRepository<Track>()
            .Include(x => x.Artist)
            .Include(x => x.Album)
            .Where(x => x.UserId == user.Id)
            .ToArrayAsync();

        return View(tracks);
    }

    [HttpPost]
    public async Task<IActionResult> AddTrack(IFormFile file)
    {
        var user = await _userManager.GetUserAsync(User);
        ViewBag.Message = await Track.AddTrack(_unitOfWork, file, user);

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> DeleteTrack(long trackId)
    {
        await Track.DeleteTrack(_unitOfWork, trackId);

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> AddTrackToPlaylist(long[] playlistIds, long trackId)
    {
        var oldPlaylists = await _unitOfWork.GetRepository<PlaylistTrack>()
            .Where(x => x.TrackId == trackId)
            .ToArrayAsync();
        
        _unitOfWork.GetRepository<PlaylistTrack>().DeleteRange(oldPlaylists);
        
        playlistIds
            .ToList()
            .ForEach(x => _unitOfWork.GetRepository<PlaylistTrack>().Add(new PlaylistTrack
            {
                PlaylistId = x,
                TrackId = trackId
            }));

        await _unitOfWork.SaveChanges();

        return RedirectToAction("Index");
    }
}