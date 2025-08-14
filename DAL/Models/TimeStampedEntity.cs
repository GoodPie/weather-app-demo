using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

public abstract class TimeStampedEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdatedAt { get; set; }
}