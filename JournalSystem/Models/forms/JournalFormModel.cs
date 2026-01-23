using System.ComponentModel.DataAnnotations;

namespace JournalSystem.Models;

public class JournalFormModel
{
    [Required(ErrorMessage = "Title is required.")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Select a category.")]
    public int Category { get; set; }

    [MinLength(1, ErrorMessage = "Add at least one mood.")]
    [MaxLength(3, ErrorMessage = "Add at most 3 moods.")]
    public List<int> ChosenMoods { get; set; } = new();

    public List<int> ChosenTags { get; set; } = new();
    public DateTime EntryDate { get; set; } = DateTime.Today;
}