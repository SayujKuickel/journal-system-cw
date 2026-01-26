using JournalSystem.Models;

namespace JournalSystem.Services;

public interface ITagService
{
    Task<List<Tag>> GetItemsAsync();
    Task<Tag> GetItemAsync(int id);
    Task<bool> CreateTagAsync(string name);
}
