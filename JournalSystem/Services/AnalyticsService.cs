using JournalSystem.Components.Pages.Auth;
using JournalSystem.Models;
using JournalSystem.Services.Interface;
using SQLite;
namespace JournalSystem.Services;

public sealed class MoodDistributionResult
{
    public int Positive { get; init; }
    public int Neutral { get; init; }
    public int Negative { get; init; }
    public int Total => Positive + Neutral + Negative;
}

public sealed class TopTagsResult
{
    public Tag Tag { get; init; }
    public int Count { get; init; }
}


public class CategoryBreakdownResult
{
    public string CategoryName { get; set; }
    public int Count { get; set; }
    public double Percentage { get; set; }
}


public class AnalyticsService : IAnalyticsService
{
    private async Task<SQLiteAsyncConnection> Db()
    => await DatabaseService.GetConnectionAsync();

    readonly JournalService journalService;
    readonly TagService tagService;

    public AnalyticsService()
    {
        tagService = new();
        journalService = new();
    }



    // 


    public async Task<MoodDistributionResult> GetMoodDistributionStats()
    {
        // Fetch all entries
        List<JournalEntry> entries = await journalService.GetAllItems();
        if (entries.Count == 0)
        {
            Console.WriteLine("No journal entries found.");
            return new MoodDistributionResult();
        }

        // Fetch all moods
        var db = await Db();
        var allMoods = await db.Table<Mood>().ToListAsync();

        int PositiveCount = 0;
        int NeutralCount = 0;
        int NegativeCount = 0;

        foreach (var entry in entries)
        {
            var primaryMood = allMoods.Find(m => m.Id == entry.PrimaryMood);
            if (primaryMood != null)
            {
                switch (primaryMood.Type)
                {
                    case Mood.MoodType.Positive: PositiveCount++; break;
                    case Mood.MoodType.Neutral: NeutralCount++; break;
                    case Mood.MoodType.Negative: NegativeCount++; break;
                }
            }

            var secondaryMoodIds = await journalService.GetSecondaryMoodsAsync(entry.Id);
            foreach (var smId in secondaryMoodIds)
            {
                var mood = allMoods.Find(m => m.Id == smId);
                if (mood == null) continue;

                switch (mood.Type)
                {
                    case Mood.MoodType.Positive: PositiveCount++; break;
                    case Mood.MoodType.Neutral: NeutralCount++; break;
                    case Mood.MoodType.Negative: NegativeCount++; break;
                }
            }
        }

        int TotalCount = PositiveCount + NeutralCount + NegativeCount;

        return new MoodDistributionResult
        {
            Positive = PositiveCount,
            Neutral = NeutralCount,
            Negative = NegativeCount
        };
    }


    public async Task<List<TopTagsResult>> GetTopUsedTags()
    {
        var db = await Db();

        var usage = await db.Table<JournalEntryTag>().ToListAsync();
        if (usage.Count == 0)
            return [];

        var grouped = usage
            .GroupBy(t => t.TagId)
            .Select(g => new { TagId = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToList();

        var tags = await db.Table<Tag>().ToListAsync();

        return grouped
            .Select(g => new TopTagsResult
            {
                Tag = tags.First(t => t.Id == g.TagId),
                Count = g.Count
            })
            .ToList();
    }

    public async Task<List<CategoryBreakdownResult>> GetCategoryBreakdown()
    {
        var db = await Db();

        var totalCount = await db.Table<JournalEntry>().CountAsync();
        if (totalCount == 0)
            return new List<CategoryBreakdownResult>();

        var sql = @"
        SELECT c.Name AS CategoryName, COUNT(*) AS Count
        FROM JournalEntry e
        JOIN Category c ON e.Category = c.Id
        GROUP BY c.Id
        ORDER BY Count DESC
    ";

        var counts = await db.QueryAsync<CategoryBreakdownResult>(sql);

        foreach (var item in counts)
        {
            item.Percentage = (double)item.Count / totalCount * 100;
        }

        return counts.ToList();
    }


}