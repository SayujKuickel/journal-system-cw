namespace JournalSystem.Services;

public class FilterProvider
{
    public int PageSize = 6;
    public int Page = 1;
    public bool HasNextPage = false;
    public string Query = string.Empty;
    public DateTime? StartDate;
    public DateTime? EndDate;
    public int SelectedTag;
    public int SelectedCategory;
    public int SelectedMood;
    public int SelectedSecondaryMood;
}

