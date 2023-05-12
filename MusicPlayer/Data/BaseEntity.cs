using System.ComponentModel.DataAnnotations;

namespace MusicPlayer.Data;

/// <summary>
/// Базовый класс для сущностей
/// </summary>
public abstract class BaseEntity : IBaseEntity
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    [Key] 
    public long Id { get; set; }
}