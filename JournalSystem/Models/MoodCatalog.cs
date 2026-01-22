using System;
using System.Collections.Generic;

namespace JournalSystem.Models
{
    public enum MoodCategory
    {
        Positive,
        Neutral,
        Negative
    }

    public enum Mood
    {
        Happy,
        Excited,
        Relaxed,
        Grateful,
        Confident,
        Calm,
        Thoughtful,
        Curious,
        Nostalgic,
        Bored,
        Sad,
        Angry,
        Stressed,
        Lonely,
        Anxious
    }

    public static class MoodCatalog
    {
        public static IReadOnlyList<Mood> Positive { get; } = new[]
        {
            Mood.Happy,
            Mood.Excited,
            Mood.Relaxed,
            Mood.Grateful,
            Mood.Confident
        };

        public static IReadOnlyList<Mood> Neutral { get; } = new[]
        {
            Mood.Calm,
            Mood.Thoughtful,
            Mood.Curious,
            Mood.Nostalgic,
            Mood.Bored
        };

        public static IReadOnlyList<Mood> Negative { get; } = new[]
        {
            Mood.Sad,
            Mood.Angry,
            Mood.Stressed,
            Mood.Lonely,
            Mood.Anxious
        };

        public static IReadOnlyList<Mood> All { get; } = new[]
        {
            Mood.Happy,
            Mood.Excited,
            Mood.Relaxed,
            Mood.Grateful,
            Mood.Confident,
            Mood.Calm,
            Mood.Thoughtful,
            Mood.Curious,
            Mood.Nostalgic,
            Mood.Bored,
            Mood.Sad,
            Mood.Angry,
            Mood.Stressed,
            Mood.Lonely,
            Mood.Anxious
        };

        public static MoodCategory GetCategory(Mood mood)
        {
            return mood switch
            {
                Mood.Happy => MoodCategory.Positive,
                Mood.Excited => MoodCategory.Positive,
                Mood.Relaxed => MoodCategory.Positive,
                Mood.Grateful => MoodCategory.Positive,
                Mood.Confident => MoodCategory.Positive,
                Mood.Calm => MoodCategory.Neutral,
                Mood.Thoughtful => MoodCategory.Neutral,
                Mood.Curious => MoodCategory.Neutral,
                Mood.Nostalgic => MoodCategory.Neutral,
                Mood.Bored => MoodCategory.Neutral,
                Mood.Sad => MoodCategory.Negative,
                Mood.Angry => MoodCategory.Negative,
                Mood.Stressed => MoodCategory.Negative,
                Mood.Lonely => MoodCategory.Negative,
                Mood.Anxious => MoodCategory.Negative,
                _ => MoodCategory.Neutral
            };
        }
    }
}
