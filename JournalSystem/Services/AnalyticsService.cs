using JournalSystem.Components.Pages.Auth;
using JournalSystem.Models;
using JournalSystem.Services.Interface;
using SQLite;
using System.Net;
using System.Text.RegularExpressions;
namespace JournalSystem.Services;

public class AnalyticsService : IAnalyticsService
{
    private async Task<SQLiteAsyncConnection> Db()
    => await DatabaseService.GetConnectionAsync();

    private static readonly Regex HtmlTagRegex =
        new(@"<[^>]+>", RegexOptions.Singleline);

    private static readonly Regex WordRegex =
        new(@"[\p{L}\p{Nd}]+", RegexOptions.CultureInvariant);

    readonly JournalService journalService;

    public AnalyticsService()
    {
        journalService = new();
    }

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
            .Take(8)
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


    public async Task<StreakSummary> GetStreaksAsync()
    {
        var db = await Db();
        var entries = await db.Table<JournalEntry>().ToListAsync();

        var dates = entries
            .Select(e => e.EntryDate.Date)
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        if (dates.Count == 0)
            return new StreakSummary
            {
                MissedDays = new List<DateTime>()
            };

        int longestCount = 1;
        DateTime longestStart = dates[0];
        DateTime longestEnd = dates[0];

        int tempCount = 1;
        DateTime tempStart = dates[0];

        var missedDays = new List<DateTime>();

        for (int i = 1; i < dates.Count; i++)
        {
            var gap = (dates[i] - dates[i - 1]).Days;

            if (gap == 1)
            {
                tempCount++;

                if (tempCount > longestCount)
                {
                    longestCount = tempCount;
                    longestStart = tempStart;
                    longestEnd = dates[i];
                }
            }
            else if (gap > 1)
            {
                for (int j = 1; j < gap; j++)
                {
                    missedDays.Add(dates[i - 1].AddDays(j));
                }

                tempCount = 1;
                tempStart = dates[i];
            }
        }

        int currentCount = 0;
        DateTime? currentEnd = null;
        DateTime? currentStart = null;
        var today = DateTime.Today;

        for (int i = dates.Count - 1; i >= 0; i--)
        {
            var diff = (today - dates[i]).Days;

            if (diff == currentCount || (currentCount == 0 && diff == 1))
            {
                currentCount++;
                currentEnd ??= dates[i];
                currentStart = dates[i];
            }
            else
            {
                break;
            }
        }

        return new StreakSummary
        {
            CurrentCount = currentCount,
            CurrentStart = currentStart,
            CurrentEnd = currentEnd,
            LongestCount = longestCount,
            LongestStart = longestStart,
            LongestEnd = longestEnd,
            MissedDays = missedDays
        };
    }

    public async Task<List<WordCountResult>> GetTopWordsAsync(int top = 10)
    {
        var safeTop = Math.Clamp(top, 1, 100);

        var db = await Db();
        var entries = await db.Table<JournalEntry>().ToListAsync();
        if (entries.Count == 0) return [];

        var frequencies = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var entry in entries)
        {
            var plainText = ExtractPlainTextFromHtml(entry.RichText);
            if (plainText.Length == 0)
                continue;

            foreach (Match match in WordRegex.Matches(plainText))
            {
                var word = match.Value.ToLowerInvariant();

                if (!frequencies.TryAdd(word, 1))
                    frequencies[word]++;
            }
        }

        if (frequencies.Count == 0)
            return [];

        return frequencies
            .OrderByDescending(el => el.Value)
            .ThenBy(el => el.Key, StringComparer.OrdinalIgnoreCase)
            .Take(safeTop)
            .Select(el => new WordCountResult { Name = el.Key, WordCount = el.Value })
            .ToList();
    }

    private static string ExtractPlainTextFromHtml(string? html)
    {
        if (string.IsNullOrWhiteSpace(html)) return string.Empty;

        var withoutTags = HtmlTagRegex.Replace(html, " ");
        return WebUtility.HtmlDecode(withoutTags).Replace('\u00A0', ' ').Trim();
    }
}
