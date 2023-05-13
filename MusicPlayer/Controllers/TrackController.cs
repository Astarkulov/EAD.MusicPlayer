using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
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
            .Include(x => x.Playlist)
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}