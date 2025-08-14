using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Index(nameof(SearchTerm))]
public class GeocodeSearch : TimeStampedEntity
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public required string SearchTerm { get; set; }
    
    public DateTime SearchedAt { get; set; }
    
    // Number of results found for this search
    public int ResultCount { get; set; }
}