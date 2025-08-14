using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

public abstract class TimeStampedEntity
{
    [Timestamp]
    public DateTime CreatedAt { get; set; }
    
    [Timestamp]
    [ConcurrencyCheck]
    public DateTime UpdatedAt { get; set; }
}