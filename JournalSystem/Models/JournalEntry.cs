using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JournalSystem.Models
{
    public enum MoodCategory
    {
        Positive,
        Neutral,
        Negative
    }

    public enum JournalTag
    {
        Work,
        Career,
        Studies,
        Family,
        Friends,
        Relationships,
        Health,
        Fitness,
        PersonalGrowth,
        SelfCare,
        Hobbies,
        Travel,
        Nature,
        Finance,
        Spirituality,
        Birthday,
        Holiday,
        Vacation,
        Celebration,
        Exercise,
        Reading,
        Writing,
        Cooking,
        Meditation,
        Yoga,
        Music,
        Shopping,
        Parenting,
        Projects,
        Planning,
        Reflection
    }

    public class JournalEntry
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public DateTime EntryDate { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public MoodCategory PrimaryMood { get; set; }

        [Ignore]
        public List<string> SecondaryMoods { get; set; } = new();
        public string SecondaryMoodsSerialized
        {
            get => string.Join(",", SecondaryMoods);
            set => SecondaryMoods = string.IsNullOrWhiteSpace(value)
                ? new List<string>()
                : value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToList();
        }

        [Ignore]
        public List<JournalTag> Tags { get; set; } = new();
        public string TagsSerialized
        {
            get => string.Join(",", Tags);
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    Tags = new List<JournalTag>();
                    return;
                }

                Tags = value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s =>
                    {
                        if (Enum.TryParse<JournalTag>(s.Trim(), out var tag))
                        {
                            return (JournalTag?)tag;
                        }

                        return null;
                    })
                    .Where(t => t.HasValue)
                    .Select(t => t!.Value)
                    .ToList();
            }
        }

        public string? Category { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

}
