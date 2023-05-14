using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicPlayer.Data;
using File = TagLib.File;

namespace MusicPlayer.Models;

/// <summary>
/// Композиция
/// </summary>
public class Track : BaseEntity
{
    /// <summary>
    /// Идентификатор альбома
    /// </summary>
    public long? AlbumId { get; set; }

    /// <summary>
    /// Идентификатор исполнителя
    /// </summary>
    public long? ArtistId { get; set; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Наименование
    /// </summary>
    [MaxLength(300)]
    public string Name { get; set; }

    /// <summary>
    /// Длительность
    /// </summary>
    public double Length { get; set; }

    /// <summary>
    /// Название файла
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Альбом
    /// </summary>
    public virtual Album Album { get; set; }

    /// <summary>
    /// Исполнитель
    /// </summary>
    public virtual Artist Artist { get; set; }

    /// <summary>
    /// Плейлист
    /// </summary>
    public virtual IList<PlaylistTrack> PlaylistTracks { get; set; }

    /// <summary>
    /// Пользователь
    /// </summary>
    public IdentityUser User { get; set; }

    public static async Task<string> AddTrack(IUnitOfWork unitOfWork, IFormFile file, IdentityUser user)
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

                var tagLibFile = File.Create(path);

                var artist = tagLibFile.Tag.FirstPerformer is not null
                    ? unitOfWork
                          .GetRepository<Artist>()
                          .Include(x => x.Albums)
                          .FirstOrDefault(
                              x => x.Name == tagLibFile.Tag.FirstPerformer && x.UserId == user.Id) ??
                      new Artist
                      {
                          UserId = user.Id,
                          Name = tagLibFile.Tag.FirstPerformer,
                          Albums = new List<Album>()
                      }
                    : null;

                var album = tagLibFile.Tag.Album is not null
                    ? unitOfWork
                          .GetRepository<Album>()
                          .FirstOrDefault(x => x.Name == tagLibFile.Tag.Album && x.UserId == user.Id)
                      ?? new Album
                      {
                          Name = tagLibFile.Tag.Album,
                          UserId = user.Id
                      }
                    : null;
                if (album is not null && album.Artist is null)
                    artist?.Albums.Add(album);

                var pictures = tagLibFile.Tag.Pictures;

                if (pictures.Length > 0 && album is not null && album.Id is null)
                {
                    var pictureData = pictures[0].Data.Data;

                    var mimeType = pictures[0].MimeType;
                    var extension = mimeType.Split('/').Last();
                    var picFilePath = Path.Combine("wwwroot", "AlbumArt", album.Name + "." + extension);
                    album.AlbumArtFileName = album.Name + "." + extension;
                    if (!System.IO.File.Exists(picFilePath))
                    {
                        await using var stream = new FileStream(picFilePath, FileMode.Create);
                        stream.Write(pictureData, 0, pictureData.Length);
                    }
                }

                var newTrack = new Track
                {
                    Name = tagLibFile.Tag.Title,
                    UserId = user.Id,
                    Length = tagLibFile.Properties.Duration.TotalSeconds,
                    Artist = artist,
                    Album = album,
                    FileName = fileName
                };

                if (artist is not null)
                {
                    if (artist.Id is not null) unitOfWork.GetRepository<Artist>().Update(artist);
                    else await unitOfWork.GetRepository<Artist>().AddAsync(artist);
                }

                if (album is not null)
                {
                    if (album.Id is not null) unitOfWork.GetRepository<Album>().Update(album);
                    else await unitOfWork.GetRepository<Album>().AddAsync(album);
                }

                await unitOfWork.GetRepository<Track>().AddAsync(newTrack);
                await unitOfWork.SaveChanges();

                result = "Трек успешно записан";
            }
            else result = "Пожалуйста, загрузите действительный аудиофайл";
        }
        else result = "Пожалуйста, выберите файл для загрузки";

        return result;
    }

    public static async Task DeleteTrack(IUnitOfWork unitOfWork, long trackId)
    {
        var track = unitOfWork.GetRepository<Track>()
            .FirstOrDefault(x => x.Id == trackId);
        if (track?.FileName != null)
        {
            var filePath = Path.Combine("wwwroot/Tracks", track.FileName);
            if (System.IO.File.Exists(filePath))
            {
                await using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                }

                await System.IO.File.WriteAllBytesAsync(filePath, await System.IO.File.ReadAllBytesAsync(filePath));
                System.IO.File.Delete(filePath);
            }
            var playlistTracks = await unitOfWork.GetRepository<PlaylistTrack>()
                .Where(x => x.TrackId == trackId)
                .ToArrayAsync();
            unitOfWork.GetRepository<PlaylistTrack>().DeleteRange(playlistTracks);
            unitOfWork.GetRepository<Track>().Delete(track);
        }
        
        await unitOfWork.SaveChanges();
    }
}