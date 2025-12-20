using System;
using System.Collections.Generic;
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
        public DateTime EntryDate { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public MoodCategory PrimaryMood { get; set; }
        public List<string> SecondaryMoods { get; set; } = new();
        public List<JournalTag> Tags { get; set; } = new();
        public string? Category { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

}
