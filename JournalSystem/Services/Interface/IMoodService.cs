using JournalSystem.Models;

namespace JournalSystem.Services;

public interface IMoodService
{
    public Task<List<Mood>> GetItemsAsync();
}