using JournalSystem.Models;
using SQLite;
using SQLitePCL;
using Windows.Web.UI;


namespace JournalSystem.Services;

public sealed class DatabaseService
{
    private static SQLiteAsyncConnection? _db;
    private static readonly SemaphoreSlim _lock = new(1, 1);
    private const string DB_NAME = "JournalSystem.db";

    public static async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_db != null) return _db;

        await _lock.WaitAsync();
        try
        {
            if (_db == null)
            {
                Batteries.Init();

                var dbPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    DB_NAME
                );

                _db = new SQLiteAsyncConnection(dbPath);

                await CreateTablesAsync(_db);
                await SeedDataAsync(_db);
            }
        }
        finally
        {
            _lock.Release();
        }

        return _db;
    }

    private static async Task CreateTablesAsync(SQLiteAsyncConnection conn)
    {
        await conn.CreateTableAsync<Category>();
        await conn.CreateTableAsync<JournalEntry>();
        await conn.CreateTableAsync<JournalEntryMood>();
        await conn.CreateTableAsync<JournalEntryTag>();
        await conn.CreateTableAsync<Mood>();
        await conn.CreateTableAsync<Tag>();
    }

    private static async Task SeedDataAsync(SQLiteAsyncConnection conn)
    {
        #region  Mood Seed
        if (await conn.Table<Mood>().CountAsync() == 0)
        {

            var moods = new[]
            {
            ("Happy", Mood.MoodType.Positive),
            ("Excited", Mood.MoodType.Positive),
            ("Relaxed", Mood.MoodType.Positive),
            ("Grateful", Mood.MoodType.Positive),
            ("Confident", Mood.MoodType.Positive),
            ("Calm", Mood.MoodType.Neutral),
            ("Thoughtful", Mood.MoodType.Neutral),
            ("Curious", Mood.MoodType.Neutral),
            ("Nostalgic", Mood.MoodType.Neutral),
            ("Bored", Mood.MoodType.Neutral),
            ("Sad", Mood.MoodType.Negative),
            ("Angry", Mood.MoodType.Negative),
            ("Stressed", Mood.MoodType.Negative),
            ("Lonely", Mood.MoodType.Negative),
            ("Anxious", Mood.MoodType.Negative),
        };

            foreach (var (name, type) in moods)
            {
                await conn.InsertAsync(new Mood
                {
                    Name = name,
                    Type = type
                });
            }
        }
        #endregion

        #region  Tag Seed
        if (await conn.Table<Tag>().CountAsync() == 0)
        {

            var tags = new List<string> { "Work", "Career", "Studies", "Family", "Friends", "Relationships", "Health", "Fitness", "Personal Growth", "Self-care", "Hobbies", "Travel", "Nature", "Finance", "Spirituality", "Birthday", "Holiday", "Vacation", "Celebration", "Exercise", "Reading", "Writing", "Cooking", "Meditation", "Yoga", "Music", "Shopping", "Parenting", "Projects", "Planning", "Reflection" };

            foreach (var tag in tags)
            {
                await conn.InsertAsync(
                    new Tag() { Name = tag }
                );
            }
        }
        #endregion


        #region  Category Seed

        if (await conn.Table<Category>().CountAsync() == 0)
        {

            List<string> categories = new() { "Work", "Health", "Travel", "Personal", };

            foreach (var category in categories)
            {
                await conn.InsertAsync(
                    new Category() { Name = category }
                );
            }

        }

        #endregion


    }
}
