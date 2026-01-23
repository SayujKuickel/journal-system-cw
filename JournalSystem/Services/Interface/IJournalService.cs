using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JournalSystem.Models;

namespace JournalSystem.Services.Interface
{
    public interface IJournalService
    {
        Task<List<JournalEntry>> GetItemsAsync(int page = 0, int perPage = 10);
        Task<JournalEntry> GetItemAsync(Guid id);
        Task<JournalEntry> GetItemByDateAsync(DateTime date);
        Task<int> SaveItemAsync(JournalEntry item);
        Task<int> UpdateItemAsync(JournalEntry item);
        Task<int> DeleteItemAsync(JournalEntry item);
        Task<int> SaveJournalEntry(JournalFormModel formData, string richText);
    }
}
