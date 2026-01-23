using System.ComponentModel.DataAnnotations;
using SQLite;

namespace JournalSystem.Models;

public class Mood
{

    public enum MoodType
    {
        Positive,
        Neutral,
        Negative
    }

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = String.Empty;

    [Required]
    public MoodType Type { get; set; }
}
