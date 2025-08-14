namespace DAL.Models;

public abstract class TimeStampedEntity
{
    public DateTime CreatedAt;
    public DateTime UpdatedAt;
}