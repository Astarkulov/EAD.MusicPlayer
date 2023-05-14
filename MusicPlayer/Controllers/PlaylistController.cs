using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicPlayer.Data;
using MusicPlayer.Dto;
using MusicPlayer.Models;
using MusicPlayer.ViewModels;
using TagLib.Riff;

namespace MusicPlayer.Controllers;

public class PlaylistController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<IdentityUser> _userManager;

    public PlaylistController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var playlists = await _unitOfWork.GetRepository<Playlist>()
            .Where(x => x.UserId == user.Id)
            .ToArrayAsync();

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
        var playlist = _unitOfWork.GetRepository<Playlist>().FirstOrDefault(x => x.Id == playlistId);
        ViewBag.PlaylistId = playlistId;
        ViewBag.PlaylistName = playlist.Name;
        return View("AddPlaylist");
    }

    [HttpPost]
    public async Task<IActionResult> SavePlaylist(string playlistName, IFormFile imageFile, long? playlistId)
    {
        var fileName = string.Empty;
        if (imageFile is not null)
        {
            if (!IsImageFile(imageFile.FileName))
                ModelState.AddModelError("image", "Некорректный формат файла. Пожалуйста, выберите изображение.");
            if (!ModelState.IsValid) return RedirectToAction("Index");
            fileName = Path.GetFileName(imageFile.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "PlaylistArt", fileName);
            await using var fileStream = new FileStream(filePath, FileMode.Create);
            await imageFile.CopyToAsync(fileStream);
        }

        var user = await _userManager.GetUserAsync(User);
        if (string.IsNullOrEmpty(playlistName)) return RedirectToAction("Index");
        if (playlistId is not null)
        {
            var playlist = _unitOfWork.GetRepository<Playlist>()
                .FirstOrDefault(x => x.Id == playlistId);
            playlist.Name = playlistName;
            playlist.PlaylistArtFileName = fileName == string.Empty ? playlist.PlaylistArtFileName : fileName;
            _unitOfWork.GetRepository<Playlist>().Update(playlist);
        }
        else
        {
            _unitOfWork.GetRepository<Playlist>().Add(new Playlist
            {
                Name = playlistName,
                UserId = user.Id,
                PlaylistArtFileName = fileName
            });
        }
        
        await _unitOfWork.SaveChanges();

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> OpenPlaylist(long? playlistId)
    {
        var playlist = _unitOfWork
            .GetRepository<Playlist>()
            .Include(x => x.PlaylistTracks)
            .FirstOrDefault(x => x.Id == playlistId);

        var tracks = await _unitOfWork.GetRepository<Track>()
            .Where(x => playlist.PlaylistTracks.Select(y => y.TrackId).Contains(x.Id))
            .ToArrayAsync();

        var playlistViewModel = new PlaylistViewModel
        {
            Name = playlist.Name,
            Id = playlist.Id,
            PlaylistArtFileName = playlist.PlaylistArtFileName,
            Tracks = tracks
        };

        return View(playlistViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> GetPlaylists(long? trackId)
    {
        var user = await _userManager.GetUserAsync(User);
        
        var checkedPlaylists = await _unitOfWork.GetRepository<PlaylistTrack>()
            .Where(x => x.TrackId == trackId)
            .Select(x => x.PlaylistId)
            .ToArrayAsync();

        var allPlaylists = await _unitOfWork.GetRepository<Playlist>()
            .Where(x => x.UserId == user.Id)
            .ToArrayAsync();

        var playlists = allPlaylists
            .Select(playlist => 
                new PlaylistData
                {
                    Name = playlist.Name, Id = playlist.Id, CheckedPlaylist = checkedPlaylists.Contains(playlist.Id)
                }).ToList();

        return Ok(playlists);
    }

    [HttpPost]
    public async Task<IActionResult> DeletePlaylist(long playlistId)
    {
        var playlist = _unitOfWork.GetRepository<Playlist>()
            .FirstOrDefault(x => x.Id == playlistId);

        var playlistTracks = await _unitOfWork.GetRepository<PlaylistTrack>()
            .Where(x => x.PlaylistId == playlistId)
            .ToArrayAsync();
        
        if (playlist != null)
        {
            _unitOfWork.GetRepository<PlaylistTrack>().DeleteRange(playlistTracks);
            _unitOfWork.GetRepository<Playlist>().Delete(playlist);
        }

        await _unitOfWork.SaveChanges();

        return RedirectToAction("Index");
    }
    
    private static bool IsImageFile(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return !string.IsNullOrEmpty(extension) && (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" ||
                                                    extension.ToLower() == ".png" || extension.ToLower() == ".gif");
    }
}