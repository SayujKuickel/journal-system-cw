using JournalSystem.Models;
using SQLite;

namespace JournalSystem.Services;

public class JournalExportService : IJournalExportService
{
    private async Task<SQLiteAsyncConnection> Db()
        => await DatabaseService.GetConnectionAsync();

    public async Task<List<JournalExportDto>> BuildJournalExport(IEnumerable<Guid> entryIds)
    {
        var ids = entryIds.Distinct().ToList();
        if (ids.Count == 0)
            return [];

        var db = await Db();

        var entries = await db.Table<JournalEntry>()
            .Where(e => ids.Contains(e.Id))
            .ToListAsync();

        if (entries.Count == 0)
            return [];

        var moods = await db.Table<Mood>().ToListAsync();
        var tags = await db.Table<Tag>().ToListAsync();
        var categories = await db.Table<Category>().ToListAsync();

        var entryTags = await db.Table<JournalEntryTag>()
            .Where(t => ids.Contains(t.JournalEntryId))
            .ToListAsync();

        var entryMoods = await db.Table<JournalEntryMood>()
            .Where(m => ids.Contains(m.JournalEntryId))
            .ToListAsync();

        return entries.Select(e => new JournalExportDto
        {
            Title = e.Title,
            Date = e.EntryDate.ToString("yyyy-MM-dd"),
            Category = categories.FirstOrDefault(c => c.Id == e.Category)?.Name ?? "",
            PrimaryMood = moods.FirstOrDefault(m => m.Id == e.PrimaryMood)?.Name ?? "",
            SecondaryMoods = entryMoods
                .Where(m => m.JournalEntryId == e.Id)
                .Select(m => moods.First(x => x.Id == m.MoodId).Name)
                .ToList(),
            Tags = entryTags
                .Where(t => t.JournalEntryId == e.Id)
                .Select(t => tags.First(x => x.Id == t.TagId).Name)
                .ToList(),
            RichTextHtml = e.RichText
        }).ToList();
    }

}
