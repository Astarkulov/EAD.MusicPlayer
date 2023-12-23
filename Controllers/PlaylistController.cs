using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MusicPlayer.Services.Interfaces;

namespace MusicPlayer.Controllers;

public class PlaylistController : Controller
{
    private readonly IPlaylistService _playlistService;
    private readonly UserManager<IdentityUser> _userManager;

    public PlaylistController(IPlaylistService playlistService, UserManager<IdentityUser> userManager)
    {
        _playlistService = playlistService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var playlists = await _playlistService.GetAllPlaylists(user);


        return View(playlists);
    }

    [HttpPost]
    public IActionResult AddPlaylist()
    {
        return View();
    }

    [HttpPost]
    public IActionResult EditPlaylist(long? playlistId)
    {
        var playlist = _playlistService.GetPlaylistById(playlistId);
        ViewBag.PlaylistId = playlistId;
        ViewBag.PlaylistName = playlist.Name;
        return View("AddPlaylist");
    }

    [HttpPost]
    public async Task<IActionResult> SavePlaylist(string playlistName, IFormFile imageFile, long? playlistId)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            await _playlistService.SavePlaylist(playlistName, imageFile, playlistId, user);
        }
        catch (Exception e)
        {
            return RedirectToAction("Index");
        }

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> OpenPlaylist(long? playlistId)
    {
        var playlistViewModel = await _playlistService.GetPlaylistTracks(playlistId);

        return View(playlistViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> GetPlaylists(long? trackId)
    {
        var user = await _userManager.GetUserAsync(User);
        var playlists = await _playlistService.GetPlaylistsForModal(trackId, user);

        return Ok(playlists);
    }

    [HttpPost]
    public async Task<IActionResult> DeletePlaylist(long playlistId)
    {
        await _playlistService.DeletePlaylist(playlistId);

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> DeleteTrackFromPlaylist(long? trackId, long playlistId)
    {
        await _playlistService.DeletePlaylistFromPlaylist(trackId, playlistId);

        return RedirectToAction("OpenPlaylist", new { playlistId });
    }
}