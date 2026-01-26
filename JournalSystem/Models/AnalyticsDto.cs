namespace JournalSystem.Models;

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

public sealed class StreakInfo
{
    public int Count { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
}

public sealed class StreakSummary
{
    public int CurrentCount { get; init; }
    public DateTime? CurrentStart { get; init; }
    public DateTime? CurrentEnd { get; init; }
    public int LongestCount { get; init; }
    public DateTime? LongestStart { get; init; }
    public DateTime? LongestEnd { get; init; }
    public List<DateTime> MissedDays { get; init; } = new();
}

public class WordCountTrendResult
{
    public DateTime Date { get; set; }
    public double AverageWords { get; set; }
}

