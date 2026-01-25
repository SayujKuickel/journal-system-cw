using SQLite;

namespace JournalSystem.Models;

public class Tag
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }

    [Unique] public string Name { get; set; }
}