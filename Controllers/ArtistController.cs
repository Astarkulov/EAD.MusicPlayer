using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MusicPlayer.Services.Interfaces;

namespace MusicPlayer.Controllers;

public class ArtistController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IArtistService _artistService;

    public ArtistController(UserManager<IdentityUser> userManager,
        IArtistService artistService)
    {
        _userManager = userManager;
        _artistService = artistService;
    }

    public async Task<IActionResult> Index()
    {
        var albums = await _artistService.GetArtists();

        return View(albums);
    }

    public async Task<IActionResult> OpenArtist(long artistId)
    {
        var artist = await _artistService.GetArtistById(artistId);

        if (artist is null) return View();

        ViewBag.ArtistArtFileName = artist.ArtistArtFileName;
        ViewBag.ArtistName = artist.Name;

        return View(artist.Tracks.ToArray());
    }
}