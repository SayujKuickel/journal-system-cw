using SQLite;

namespace JournalSystem.Services.Interface;

public interface IDatabaseService
{
    Task<SQLiteAsyncConnection> GetConnectionAsync();
}

