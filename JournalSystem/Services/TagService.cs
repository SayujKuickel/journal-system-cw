using JournalSystem.Models;
using SQLite;

namespace JournalSystem.Services;

public class TagService : ITagService
{
    private async Task<SQLiteAsyncConnection> Db()
        => await DatabaseService.GetConnectionAsync();

    // get tag
    public async Task<List<Tag>> GetItemsAsync()
    {
        var db = await Db();

        return await db.Table<Tag>().ToListAsync();
    }

    // get tag with id
    public async Task<Tag> GetItemAsync(int id)
    {
        var db = await Db();

        return await db.Table<Tag>().FirstOrDefaultAsync(e => e.Id == id);
    }

    // create new tag
    public async Task<bool> CreateTagAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        name = name.Trim();

        var db = await Db();

        var exists = await db.Table<Tag>()
            .Where(t => t.Name == name)
            .FirstOrDefaultAsync();

        if (exists != null)
            return false;

        await db.InsertAsync(new Tag { Name = name });
        return true;
    }
}
