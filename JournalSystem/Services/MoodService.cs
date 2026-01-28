using JournalSystem.Models;
using SQLite;

namespace JournalSystem.Services;

public class MoodService : IMoodService
{
    private async Task<SQLiteAsyncConnection> Db()
        => await DatabaseService.GetConnectionAsync();

    // get items
    public async Task<List<Mood>> GetItemsAsync()
    {
        var db = await Db();

        return await db.Table<Mood>().ToListAsync();
    }
}
