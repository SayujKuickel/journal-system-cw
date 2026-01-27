using JournalSystem.Models;

namespace JournalSystem.Services.Interface;

public interface IAnalyticsService
{
    Task<MoodDistributionResult> GetMoodDistributionStats();
    Task<List<TopTagsResult>> GetTopUsedTags();
    Task<List<CategoryBreakdownResult>> GetCategoryBreakdown();
    Task<StreakSummary> GetStreaksAsync();
    Task<List<WordCountResult>> GetTopWordsAsync(int top = 10);
}
