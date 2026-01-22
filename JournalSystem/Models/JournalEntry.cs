using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JournalSystem.Models
{
    public class JournalEntry
    {
        [PrimaryKey]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public DateTime EntryDate { get; set; }

        [Indexed(Unique = true)]
        public string EntryDay { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");
        [Required]
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        [Required]
        public Mood PrimaryMood { get; set; }

        [Ignore]
        public List<Mood> SecondaryMoods { get; set; } = new();
        public string SecondaryMoodsSerialized
        {
            get => string.Join(",", SecondaryMoods);
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    SecondaryMoods = new List<Mood>();
                    return;
                }

                SecondaryMoods = value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s =>
                    {
                        if (Enum.TryParse<Mood>(s.Trim(), out var mood))
                        {
                            return (Mood?)mood;
                        }

                        return null;
                    })
                    .Where(m => m.HasValue)
                    .Select(m => m!.Value)
                    .Take(2)
                    .ToList();
            }
        }

        [Ignore]
        public List<string> Tags { get; set; } = new();
        public string TagsSerialized
        {
            get => string.Join(",", Tags);
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    Tags = new List<string>();
                    return;
                }

                Tags = value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(TagCatalog.NormalizeTag)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
        }

        public string? Category { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

}
