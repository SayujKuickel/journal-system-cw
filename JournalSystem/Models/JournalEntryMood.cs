using SQLite;

namespace JournalSystem.Models;

public class JournalEntryMood
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public Guid JournalEntryId { get; set; }

    public int MoodId { get; set; }
}