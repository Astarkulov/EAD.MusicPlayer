using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MusicPlayer.Services.Interfaces;

namespace MusicPlayer.Controllers;

public class TrackController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ITrackService _trackService;

    public TrackController(UserManager<IdentityUser> userManager,
        ITrackService trackService)
    {
        _userManager = userManager;
        _trackService = trackService;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var tracks = await _trackService.GetAllTracks(user);

        return View(tracks);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetFilteredTracks(string searchText)
    {
        var user = await _userManager.GetUserAsync(User);
        var tracks = await _trackService.GetFilteredTracks(user, searchText);

        return View("Index", tracks);
    }

    [HttpPost]
    public async Task<IActionResult> AddTrack(IFormFile file)
    {
        var user = await _userManager.GetUserAsync(User);
        ViewBag.Message = await _trackService.AddTrack(file, user);

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> DeleteTrack(long trackId)
    {
        await _trackService.DeleteTrack(trackId);

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> AddTrackToPlaylist(long[] playlistIds, long trackId)
    {
        await _trackService.AddTrackToPlaylist(playlistIds, trackId);

        return RedirectToAction("Index");
    }
}