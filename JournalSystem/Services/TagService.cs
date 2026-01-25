using JournalSystem.Models;
using SQLite;

namespace JournalSystem.Services;

public class TagService : ITagService
{
    private async Task<SQLiteAsyncConnection> Db()
        => await DatabaseService.GetConnectionAsync();

    public async Task<List<Tag>> GetItemsAsync()
    {
        var db = await Db();

        return await db.Table<Tag>().ToListAsync();
    }

    public async Task<Tag> GetItemAsync(int id)
    {
        var db = await Db();

        return await db.Table<Tag>().FirstOrDefaultAsync(e => e.Id == id);
    }
}
