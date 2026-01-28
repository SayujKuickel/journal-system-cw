using SQLite;

namespace JournalSystem.Services;

public class CategoryService : ICategoryService
{
    private async Task<SQLiteAsyncConnection> Db()
        => await DatabaseService.GetConnectionAsync();
    private ToastService toast;

    public CategoryService()
    {
        toast = new();
    }

    // GET
    public async Task<List<Category>> GetItemsAsync()
    {
        var db = await Db();

        return await db.Table<Category>().ToListAsync();
    }
    // create 
    public async Task<bool> CreateCategoryAsync(string name)
    {
        var db = await Db();

        name = name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            return false;

        var exists = await db.Table<Category>()
            .Where(c => c.Name == name)
            .FirstOrDefaultAsync();

        if (exists != null)
            return false;

        await db.InsertAsync(new Category { Name = name });
        return true;
    }


}
