using System;
using System.Collections.Generic;

namespace JournalSystem.Models
{
    public static class TagCatalog
    {
        public static IReadOnlyList<string> PredefinedTags { get; } = new[]
        {
            "Work",
            "Career",
            "Studies",
            "Family",
            "Friends",
            "Relationships",
            "Health",
            "Fitness",
            "Personal Growth",
            "Self-care",
            "Hobbies",
            "Travel",
            "Nature",
            "Finance",
            "Spirituality",
            "Birthday",
            "Holiday",
            "Vacation",
            "Celebration",
            "Exercise",
            "Reading",
            "Writing",
            "Cooking",
            "Meditation",
            "Yoga",
            "Music",
            "Shopping",
            "Parenting",
            "Projects",
            "Planning",
            "Reflection"
        };

        public static string NormalizeTag(string tag)
        {
            return (tag ?? string.Empty).Trim();
        }

        public static bool IsPredefined(string tag)
        {
            tag = NormalizeTag(tag);
            if (string.IsNullOrWhiteSpace(tag))
            {
                return false;
            }

            foreach (var preset in PredefinedTags)
            {
                if (string.Equals(preset, tag, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
