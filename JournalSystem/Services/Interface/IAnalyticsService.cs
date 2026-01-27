using JournalSystem.Models;

namespace JournalSystem.Services.Interface;

public interface IAnalyticsService
{
    Task<MoodDistributionResult> GetMoodDistributionStats(DateTime? startDate = null, DateTime? endDate = null);
    Task<List<TopTagsResult>> GetTopUsedTags(DateTime? startDate = null, DateTime? endDate = null);
    Task<List<CategoryBreakdownResult>> GetCategoryBreakdown(DateTime? startDate = null, DateTime? endDate = null);
    Task<StreakSummary> GetStreaksAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<List<WordTrendResult>> GetWordTrendsAsync(DateTime? startDate = null, DateTime? endDate = null);
}
