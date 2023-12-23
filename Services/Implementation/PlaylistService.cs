using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicPlayer.Data;
using MusicPlayer.Dto;
using MusicPlayer.Models;
using MusicPlayer.Services.Interfaces;
using MusicPlayer.ViewModels;

namespace MusicPlayer.Services.Implementation;

public class PlaylistService : IPlaylistService
{
    private readonly IUnitOfWork _unitOfWork;

    public PlaylistService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Playlist[]> GetAllPlaylists(IdentityUser user)
    {
        var playlists = await _unitOfWork.GetRepository<Playlist>()
            .Where(x => x.UserId == user.Id)
            .ToArrayAsync();

        return playlists;
    }

    public Playlist GetPlaylistById(long? playlistId)
    {
        var playlist = _unitOfWork.GetRepository<Playlist>()
            .FirstOrDefault(x => x.Id == playlistId);

        return playlist;
    }

    public async Task SavePlaylist(string playlistName, IFormFile imageFile, long? playlistId, IdentityUser user)
    {
        var fileName = string.Empty;
        if (imageFile is not null)
        {
            if (!IsImageFile(imageFile.FileName))
                throw new Exception();
            fileName = Path.GetFileName(imageFile.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "PlaylistArt", fileName);
            await using var fileStream = new FileStream(filePath, FileMode.Create);
            await imageFile.CopyToAsync(fileStream);
        }

        if (string.IsNullOrEmpty(playlistName)) throw new Exception();
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
    }

    public async Task<PlaylistViewModel> GetPlaylistTracks(long? playlistId)
    {
        var playlist = _unitOfWork
            .GetRepository<Playlist>()
            .Include(x => x.PlaylistTracks)
            .FirstOrDefault(x => x.Id == playlistId);

        var tracks = await _unitOfWork.GetRepository<Track>()
            .Where(x => playlist.PlaylistTracks.Select(y => y.TrackId).Contains(x.Id))
            .Include(x => x.Album)
            .Include(x => x.Artist)
            .ToArrayAsync();

        var playlistViewModel = new PlaylistViewModel
        {
            Name = playlist.Name,
            Id = playlist.Id,
            PlaylistArtFileName = playlist.PlaylistArtFileName,
            Tracks = tracks
        };

        return playlistViewModel;
    }

    public async Task<PlaylistData[]> GetPlaylistsForModal(long? trackId, IdentityUser user)
    {
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
                }).ToArray();

        return playlists;
    }

    public async Task DeletePlaylist(long playlistId)
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
    }

    public async Task DeletePlaylistFromPlaylist(long? trackId, long playlistId)
    {
        var playlistTrack = _unitOfWork.GetRepository<PlaylistTrack>()
            .FirstOrDefault(x => x.TrackId == trackId);

        _unitOfWork.GetRepository<PlaylistTrack>().Delete(playlistTrack);

        await _unitOfWork.SaveChanges();
    }

    private static bool IsImageFile(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return !string.IsNullOrEmpty(extension) && (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" ||
                                                    extension.ToLower() == ".png" || extension.ToLower() == ".gif");
    }
}