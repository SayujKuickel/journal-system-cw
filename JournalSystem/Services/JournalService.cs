using JournalSystem.Models;
using JournalSystem.Services.Interface;
using SQLite;

namespace JournalSystem.Services;

public class JournalService : IJournalService
{
    private async Task<SQLiteAsyncConnection> Db()
        => await DatabaseService.GetConnectionAsync();

    public async Task<List<JournalEntry>> GetItemsAsync(int page = 1, int perPage = 10, string query = "")
    {
        var safePage = Math.Max(1, page);
        var safePerPage = Math.Max(1, perPage);
        var offset = (safePage - 1) * safePerPage;

        var db = await Db();
        var table = db.Table<JournalEntry>();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var str = query.Trim().ToLower();
            table = table.Where(el =>
                el.Title.ToLower().Contains(str) ||
                el.RichText.ToLower().Contains(str));
        }

        return await table
            .OrderByDescending(e => e.EntryDate)
            .Skip(offset)
            .Take(safePerPage)
            .ToListAsync();
    }


    public async Task<JournalEntry?> GetItemAsync(Guid id)
    {
        var db = await Db();

        return await db.Table<JournalEntry>().FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<JournalEntry?> GetItemByDateAsync(DateTime date)
    {
        var db = await Db();
        var dateOnly = date.Date;
        var entries = await db.Table<JournalEntry>()
                              .ToListAsync();
        return entries.FirstOrDefault(e => e.EntryDate.Date == dateOnly);
    }

    public async Task<int> SaveItemAsync(JournalEntry item)
    {
        if (item.Id == Guid.Empty)
            item.Id = Guid.NewGuid();

        if (item.EntryDate == default)
            item.EntryDate = DateTime.Now;

        var db = await Db();
        return await db.InsertAsync(item);
    }

    public async Task<int> UpdateItemAsync(JournalEntry item)
    {
        if (item.EntryDate == default)
            item.EntryDate = DateTime.Now;

        var db = await Db();
        return await db.UpdateAsync(item);
    }

    public async Task<int> DeleteItemAsync(JournalEntry item)
    {
        var db = await Db();
        return await db.DeleteAsync(item);
    }

    public async Task<int> SaveJournalEntry(JournalFormModel formData, string richText)
    {
        var entry = new JournalEntry
        {
            Id = Guid.NewGuid(),
            Title = formData.Title,
            RichText = richText,
            EntryDate = formData.EntryDate == default ? DateTime.Now : formData.EntryDate,
            PrimaryMood = formData.ChosenMoods.FirstOrDefault(),
            Category = formData.Category,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var db = await Db();
        await db.InsertAsync(entry);

        // saving secondary Moods
        foreach (var moodId in formData.ChosenMoods.Skip(1))
        {
            await db.InsertAsync(new JournalEntryMood
            {
                JournalEntryId = entry.Id,
                MoodId = moodId
            });
        }

        // Saving tags

        foreach (var tagId in formData.ChosenTags)
        {
            await db.InsertAsync(new JournalEntryTag
            {
                JournalEntryId = entry.Id,
                TagId = tagId
            });
        }
        return 1;
    }

    public async Task UpdateJournalEntry(Guid id, JournalFormModel formData, string richText)
    {
        var db = await Db();

        var entry = await db.GetAsync<JournalEntry>(id);
        if (entry == null)
            throw new InvalidOperationException("Journal entry not found.");

        // update main entry
        entry.Title = formData.Title;
        entry.RichText = richText;
        entry.PrimaryMood = formData.ChosenMoods.FirstOrDefault();
        entry.Category = formData.Category;
        entry.UpdatedAt = DateTime.Today;

        await db.UpdateAsync(entry);

        // replace secondary moods
        var existingMoods = await db.Table<JournalEntryMood>()
            .Where(m => m.JournalEntryId == id)
            .ToListAsync();

        foreach (var mood in existingMoods)
        {
            await db.DeleteAsync(mood);
        }

        foreach (var moodId in formData.ChosenMoods.Skip(1))
        {
            await db.InsertAsync(new JournalEntryMood
            {
                JournalEntryId = id,
                MoodId = moodId
            });
        }

        // replace tags
        var existingTags = await db.Table<JournalEntryTag>()
            .Where(t => t.JournalEntryId == id)
            .ToListAsync();

        foreach (var tag in existingTags)
        {
            await db.DeleteAsync(tag);
        }

        foreach (var tagId in formData.ChosenTags)
        {
            await db.InsertAsync(new JournalEntryTag
            {
                JournalEntryId = id,
                TagId = tagId
            });
        }
    }

    public async Task<List<int>> GetSecondaryMoodsAsync(Guid id)
    {
        var db = await Db();

        var moods = await db.Table<JournalEntryMood>().Where(el => el.JournalEntryId == id).ToListAsync();

        return moods.Select(el => el.MoodId).ToList();
    }

    public async Task<List<int>> GetTagsAsync(Guid id)
    {
        var db = await Db();

        var tags = await db.Table<JournalEntryTag>().Where(el => el.JournalEntryId == id).ToListAsync();

        return tags.Select(el => el.TagId).ToList();
    }

}
