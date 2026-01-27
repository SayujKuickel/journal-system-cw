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

    private async Task<List<JournalEntry>> GetEntriesDateRangeAsync(DateTime? startDate, DateTime? endDate)
    {
        var db = await Db();

        var q = db.Table<JournalEntry>();

        if (startDate.HasValue)
        {
            var start = startDate.Value.Date;
            q = q.Where(e => e.EntryDate >= start);
        }

        if (endDate.HasValue)
        {
            var endExclusive = endDate.Value.Date.AddDays(1);
            q = q.Where(e => e.EntryDate < endExclusive);
        }

        return await q.ToListAsync();
    }


    public async Task<MoodDistributionResult> GetMoodDistributionStats(DateTime? startDate = null, DateTime? endDate = null)
    {
        var entries = await GetEntriesDateRangeAsync(startDate, endDate);
        if (entries.Count == 0)
        {
            Console.WriteLine("No journal entries found.");
            return new MoodDistributionResult();
        }

        var db = await Db();
        var allMoods = await db.Table<Mood>().ToListAsync();

        var entryIdSet = entries.Select(e => e.Id).ToHashSet();
        var secondaryMoods = await db.Table<JournalEntryMood>().ToListAsync();
        var secondaryMoodsByEntry = secondaryMoods
            .Where(m => entryIdSet.Contains(m.JournalEntryId))
            .GroupBy(m => m.JournalEntryId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.MoodId).ToList());

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

            if (!secondaryMoodsByEntry.TryGetValue(entry.Id, out var secondaryMoodIds)) continue;

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

    public async Task<List<TopTagsResult>> GetTopUsedTags(DateTime? startDate = null, DateTime? endDate = null)
    {
        var db = await Db();

        var usage = await db.Table<JournalEntryTag>().ToListAsync();
        if (usage.Count == 0)
            return [];

        if (startDate.HasValue || endDate.HasValue)
        {
            var entries = await GetEntriesDateRangeAsync(startDate, endDate);
            if (entries.Count == 0)
                return [];

            var entryIdSet = entries.Select(e => e.Id).ToHashSet();
            usage = usage.Where(u => entryIdSet.Contains(u.JournalEntryId)).ToList();
            if (usage.Count == 0)
                return [];
        }

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

    public async Task<List<CategoryBreakdownResult>> GetCategoryBreakdown(DateTime? startDate = null, DateTime? endDate = null)
    {
        var db = await Db();

        var entries = await GetEntriesDateRangeAsync(startDate, endDate);
        if (entries.Count == 0)
            return [];

        var categories = await db.Table<Category>().ToListAsync();
        var categoryNameById = categories.ToDictionary(c => c.Id, c => c.Name);

        var totalCount = entries.Count;

        var counts = entries
            .GroupBy(e => e.Category)
            .Select(g =>
            {
                categoryNameById.TryGetValue(g.Key, out var name);
                return new CategoryBreakdownResult
                {
                    CategoryName = name ?? "Unknown",
                    Count = g.Count()
                };
            })
            .OrderByDescending(x => x.Count)
            .ToList();

        foreach (var item in counts)
        {
            item.Percentage = (double)item.Count / totalCount * 100;
        }

        return counts.ToList();
    }


    public async Task<StreakSummary> GetStreaksAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var entries = await GetEntriesDateRangeAsync(startDate, endDate);

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

        var rangeStart = startDate ?? dates.First();
        var rangeEnd = endDate ?? dates.Last();

        int longestCount = 1;
        DateTime longestStart = dates[0];
        DateTime longestEnd = dates[0];

        int tempCount = 1;
        DateTime tempStart = dates[0];

        var missedDays = new List<DateTime>();

        for (var d = rangeStart.Date; d < dates[0].Date; d = d.AddDays(1))
            missedDays.Add(d);

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

        for (var d = dates[^1].Date.AddDays(1); d <= rangeEnd.Date; d = d.AddDays(1))
            missedDays.Add(d);

        int currentCount = 0;
        DateTime? currentEnd = null;
        DateTime? currentStart = null;
        var effectiveEnd = endDate ?? DateTime.Today;

        for (int i = dates.Count - 1; i >= 0; i--)
        {
            var diff = (effectiveEnd.Date - dates[i]).Days;

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

    public async Task<List<WordTrendResult>> GetWordTrendsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var entries = await GetEntriesDateRangeAsync(startDate, endDate);
        if (entries.Count == 0)
            return [];

        return entries
            .OrderBy(e => e.EntryDate)
            .Select(e =>
            {
                var plainText = ExtractPlainTextFromHtml(e.RichText);
                if (plainText.Length == 0)
                {
                    return new WordTrendResult
                    {
                        EntryId = e.Id,
                        Title = e.Title ?? "",
                        EntryDate = e.EntryDate.Date,
                        WordCount = 0,
                        TopWord = "",
                        TopWordCount = 0
                    };
                }

                var wordCount = 0;
                var frequencies = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                foreach (Match match in WordRegex.Matches(plainText))
                {
                    var word = match.Value.ToLowerInvariant();
                    wordCount++;

                    if (!frequencies.TryAdd(word, 1))
                        frequencies[word]++;
                }

                var topWord = "";
                var topWordCount = 0;

                if (frequencies.Count > 0)
                {
                    var top = frequencies
                        .OrderByDescending(kvp => kvp.Value)
                        .ThenBy(kvp => kvp.Key, StringComparer.OrdinalIgnoreCase)
                        .First();

                    topWord = top.Key;
                    topWordCount = top.Value;
                }

                return new WordTrendResult
                {
                    EntryId = e.Id,
                    Title = e.Title ?? "",
                    EntryDate = e.EntryDate.Date,
                    WordCount = wordCount,
                    TopWord = topWord,
                    TopWordCount = topWordCount
                };
            })
            .ToList();
    }

    private static string ExtractPlainTextFromHtml(string? html)
    {
        if (string.IsNullOrWhiteSpace(html)) return string.Empty;

        var withoutTags = HtmlTagRegex.Replace(html, " ");
        return WebUtility.HtmlDecode(withoutTags).Replace('\u00A0', ' ').Trim();
    }
}
