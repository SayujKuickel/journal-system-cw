using SQLite;

namespace JournalSystem.Models;

public class JournalEntryTag
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public Guid JournalEntryId { get; set; }

    public int TagId { get; set; }
}