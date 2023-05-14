using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicPlayer.Data;
using MusicPlayer.Models;
using MusicPlayer.Services.Interfaces;
using TagLib;
using static TagLib.File;
using File = System.IO.File;

namespace MusicPlayer.Services.Implementation;

public class TrackService : ITrackService
{
    private readonly IUnitOfWork _unitOfWork;

    public TrackService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<string> AddTrack(IFormFile file)
    {
        string result;
        if (file is { Length: > 0 })
        {
            var fileName = Path.GetFileName(file.FileName);
            var fileExtension = Path.GetExtension(fileName);

            if (fileExtension is ".mp3" or ".wav")
            {
                var path = Path.Combine("wwwroot/Tracks", fileName);
                await using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var tagLibFile = Create(path);

                var artist = tagLibFile.Tag.FirstPerformer is not null
                    ? _unitOfWork
                          .GetRepository<Artist>()
                          .Include(x => x.Albums)
                          .FirstOrDefault(
                              x => x.Name == tagLibFile.Tag.FirstPerformer) ??
                      new Artist
                      {
                          Name = tagLibFile.Tag.FirstPerformer,
                          Albums = new List<Album>()
                      }
                    : null;

                var album = tagLibFile.Tag.Album is not null
                    ? _unitOfWork
                          .GetRepository<Album>()
                          .FirstOrDefault(x => x.Name == tagLibFile.Tag.Album)
                      ?? new Album
                      {
                          Name = tagLibFile.Tag.Album,
                      }
                    : null;
                if (album is not null && album.Artist == null)
                    artist?.Albums.Add(album);

                var pictures = tagLibFile.Tag.Pictures;
                if (pictures.Length > 0)
                {
                    if (album is { Id: null })
                        album.AlbumArtFileName = await SavePicture(pictures[0], album.Name);
                    if (artist is { Id: null })
                        artist.ArtistArtFileName =
                            await SavePicture(pictures.FirstOrDefault(p => p.Type == PictureType.Artist), artist.Name);
                }

                var newTrack = new Track
                {
                    Name = tagLibFile.Tag.Title,
                    Length = tagLibFile.Properties.Duration.TotalSeconds,
                    Artist = artist,
                    Album = album,
                    FileName = fileName,
                    Genres = string.Join(",", tagLibFile.Tag.Genres)
                };

                if (artist is not null)
                {
                    if (artist.Id is not null) _unitOfWork.GetRepository<Artist>().Update(artist);
                    else await _unitOfWork.GetRepository<Artist>().AddAsync(artist);
                }

                if (album is not null)
                {
                    if (album.Id is not null) _unitOfWork.GetRepository<Album>().Update(album);
                    else await _unitOfWork.GetRepository<Album>().AddAsync(album);
                }

                await _unitOfWork.GetRepository<Track>().AddAsync(newTrack);
                await _unitOfWork.SaveChanges();

                result = "Трек успешно записан";
            }
            else result = "Пожалуйста, загрузите действительный аудиофайл";
        }
        else result = "Пожалуйста, выберите файл для загрузки";

        return result;
    }

    private static async Task<string?> SavePicture(IPicture? picture, string fileName)
    {
        if (picture is null) return null;
        var pictureData = picture.Data.Data;
        var mimeType = picture.MimeType;
        var extension = mimeType.Split('/').Last();
        var picFilePath = Path.Combine("wwwroot", "AlbumArt", fileName + "." + extension);

        if (File.Exists(picFilePath)) return fileName + "." + extension;
        await using var stream = new FileStream(picFilePath, FileMode.Create);
        stream.Write(pictureData, 0, pictureData.Length);
        return fileName + "." + extension;
    }

    public async Task DeleteTrack(long trackId)
    {
        var track = _unitOfWork.GetRepository<Track>()
            .FirstOrDefault(x => x.Id == trackId);
        if (track?.FileName != null)
        {
            var filePath = Path.Combine("wwwroot/Tracks", track.FileName);
            if (File.Exists(filePath))
            {
                await using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                }

                await File.WriteAllBytesAsync(filePath, await File.ReadAllBytesAsync(filePath));
                File.Delete(filePath);
            }

            var playlistTracks = await _unitOfWork.GetRepository<PlaylistTrack>()
                .Where(x => x.TrackId == trackId)
                .ToArrayAsync();
            if (track.AlbumId is not null)
            {
                var album = _unitOfWork.GetRepository<Album>()
                    .Include(x => x.Tracks)
                    .First(x => x.Id == track.AlbumId);
                
                if(album.Tracks.Count == 1)
                    _unitOfWork.GetRepository<Album>().Delete(album);
            }

            if (track.ArtistId is not null)
            {
                var artist = _unitOfWork.GetRepository<Artist>()
                    .Include(x => x.Tracks)
                    .First(x => x.Id == track.ArtistId);
                
                if(artist.Tracks.Count == 1)
                    _unitOfWork.GetRepository<Artist>().Delete(artist);
            }
            _unitOfWork.GetRepository<PlaylistTrack>().DeleteRange(playlistTracks);
            _unitOfWork.GetRepository<Track>().Delete(track);
        }

        await _unitOfWork.SaveChanges();
    }

    public async Task AddTrackToPlaylist(IEnumerable<long> playlistIds, long trackId)
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
    }

    public async Task<Track[]> GetAllTracks()
    {
        var tracks = await _unitOfWork
            .GetRepository<Track>()
            .Include(x => x.Artist)
            .Include(x => x.Album)
            .ToArrayAsync();

        return tracks;
    }

    public async Task<Track[]> GetFilteredTracks(string searchText)
    {
        var tracks = await _unitOfWork
            .GetRepository<Track>()
            .Where(x =>
                        x.Artist.Name.Contains(searchText) ||
                        x.Album.Name.Contains(searchText) ||
                        x.Name.Contains(searchText) ||
                        x.Genres.Contains(searchText))
            .Include(x => x.Artist)
            .Include(x => x.Album)
            .ToArrayAsync();

        return tracks;
    }

    public async Task<bool> IsAdminUser(IdentityUser user)
    {
        return await _unitOfWork.IsAdminUser(user);
    }
}