using JournalSystem.Models;

namespace JournalSystem.Services;

public interface ITagService
{
    public Task<List<Tag>> GetItemsAsync();
}
