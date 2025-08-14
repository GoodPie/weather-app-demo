namespace DAL.Models;

public abstract class TimeStampedEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}