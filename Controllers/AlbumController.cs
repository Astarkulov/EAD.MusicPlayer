﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MusicPlayer.Services.Interfaces;

namespace MusicPlayer.Controllers;

public class AlbumController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IAlbumService _albumService;

    public AlbumController(UserManager<IdentityUser> userManager,
        IAlbumService albumService)
    {
        _userManager = userManager;
        _albumService = albumService;
    }

    public async Task<IActionResult> Index()
    {
        var albums = await _albumService.GetAlbums();

        return View(albums);
    }

    public async Task<IActionResult> OpenTracks(long albumId)
    {
        var album = await _albumService.GetAlbumById(albumId);

        if (album is null) return View();

        ViewBag.AlbumArtFileName = album.AlbumArtFileName;
        ViewBag.AlbumName = album.Name;

        return View(album.Tracks.ToArray());
    }
}