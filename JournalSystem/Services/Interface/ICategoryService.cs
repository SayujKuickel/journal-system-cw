namespace JournalSystem.Services;

public interface ICategoryService
{
    public Task<List<Category>> GetItemsAsync();
    Task<bool> CreateCategoryAsync(string name);
}