namespace JournalSystem.Services;

public sealed class JournalExportDto
{
    public string Title { get; init; } = "";
    public string Date { get; init; } = "";
    public string Category { get; init; } = "";
    public string PrimaryMood { get; init; } = "";
    public List<string> SecondaryMoods { get; init; } = new();
    public List<string> Tags { get; init; } = new();
    public string RichTextHtml { get; init; } = "";
}


public interface IJournalExportService
{
    Task<List<JournalExportDto>> BuildJournalExport(IEnumerable<Guid> entryIds);
}
