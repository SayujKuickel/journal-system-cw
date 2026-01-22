using JournalSystem.Constants;
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

        private SQLiteAsyncConnection Connection =>
            db ?? throw new InvalidOperationException("Database connection is not initialized.");

        async Task Init()
        {
            if (isInitialized) return;

            db = new SQLiteAsyncConnection(DbConstant.DatabasePath, DbConstant.Flags);

            await db.CreateTableAsync<JournalEntry>();
            isInitialized = true;
        }

        public async Task<List<JournalEntry>> GetItemsAsync(int page = 0, int perPage = 10)
        {
            await Init();
            var safePage = page < 0 ? 0 : page;
            var safePerPage = perPage <= 0 ? 10 : perPage;
            var offset = safePage * safePerPage;

            return await Connection.Table<JournalEntry>()
                .OrderByDescending(e => e.EntryDate)
                .Skip(offset)
                .Take(safePerPage)
                .ToListAsync();
        }

        public async Task<JournalEntry> GetItemAsync(Guid id)
        {
            await Init();
            return await Connection.Table<JournalEntry>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public async Task<JournalEntry?> GetByDayAsync(DateTime day)
        {
            await Init();
            var key = day.Date.ToString("yyyy-MM-dd");
            return await Connection.Table<JournalEntry>()
                .Where(e => e.EntryDay == key)
                .FirstOrDefaultAsync();
        }

        public async Task<int> SaveItemAsync(JournalEntry item)
        {
            await Init();
            if (item.EntryDate == default)
            {
                item.EntryDate = DateTime.Now;
            }

            item.EntryDay = item.EntryDate.Date.ToString("yyyy-MM-dd");

            return await Connection.InsertAsync(item);
        }

        public async Task<int> UpdateItemAsync(JournalEntry item)
        {
            await Init();
            if (item.EntryDate == default)
            {
                item.EntryDate = DateTime.Now;
            }

            item.EntryDay = item.EntryDate.Date.ToString("yyyy-MM-dd");

            return await Connection.UpdateAsync(item);
        }

        public async Task<int> DeleteItemAsync(JournalEntry item)
        {
            await Init();
            return await Connection.DeleteAsync(item);
        }
    }
}
