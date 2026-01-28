using System.ComponentModel.DataAnnotations;
using SQLite;

namespace JournalSystem.Models;

public class JournalEntry
{
    [PrimaryKey]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, Unique]
    public DateTime EntryDate { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public string RichText { get; set; } = string.Empty;

    public int PrimaryMood { get; set; }

    public int Category { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
