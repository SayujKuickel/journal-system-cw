using SQLite;

namespace JournalSystem.Services;

public class CategoryService : ICategoryService
{
    private async Task<SQLiteAsyncConnection> Db()
        => await DatabaseService.GetConnectionAsync();

    // GET
    public async Task<List<Category>> GetItemsAsync()
    {
        var db = await Db();

        return await db.Table<Category>().ToListAsync();
    }
}
