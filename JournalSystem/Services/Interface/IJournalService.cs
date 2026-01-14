using JournalSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JournalSystem.Services.Interface
{
    public interface IJournalService
    {
        Task<List<JournalEntry>> GetItemsAsync();
        Task<JournalEntry> GetItemAsync(Guid id);
        Task<int> SaveItemAsync(JournalEntry item);
        Task<int> DeleteItemAsync(JournalEntry item);
    }
}
