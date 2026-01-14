using JournalSystem.Models;
using JournalSystem.Services.Interface;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JournalSystem.Services
{
    public class JournalService : IJournalService
    {
        private SQLiteAsyncConnection? db;
        private bool isInitialized;

        async Task Init()
        {
            if (isInitialized) return;

            db = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);

            await db.CreateTableAsync<JournalEntry>();
            isInitialized = true;
        }

        public async Task<List<JournalEntry>> GetItemsAsync()
        {
            await Init();
            return await db.Table<JournalEntry>().ToListAsync();
        }

        public async Task<JournalEntry> GetItemAsync(Guid id)
        {
            await Init();
            return await db.Table<JournalEntry>()
                            .Where(i => i.Id == id)
                            .FirstOrDefaultAsync();
        }

        public async Task<int> SaveItemAsync(JournalEntry item)
        {
            await Init();
            if (item.Id != Guid.Empty)
            {
                return await db.UpdateAsync(item);
            }
            else
            {
                return await db.InsertAsync(item);
            }
        }

        public async Task<int> DeleteItemAsync(JournalEntry item)
        {
            await Init();
            return await db.DeleteAsync(item);
        }
    }
}
