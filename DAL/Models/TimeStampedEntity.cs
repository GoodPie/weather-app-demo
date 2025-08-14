using System.ComponentModel.DataAnnotations;

namespace DAL.Models;

public abstract class TimeStampedEntity
{
    [Timestamp] public DateTime CreatedAt { get; set; }

    [Timestamp] [ConcurrencyCheck] public DateTime UpdatedAt { get; set; }
}