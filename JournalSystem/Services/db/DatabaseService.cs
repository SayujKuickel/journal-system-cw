using JournalSystem.Models;
using SQLite;
using SQLitePCL;
using Windows.Web.UI;


namespace JournalSystem.Services;

public sealed class DatabaseService
{
    private static SQLiteAsyncConnection? _db;
    private static readonly SemaphoreSlim _lock = new(1, 1);
    private const string DB_NAME = "journal-system-abc.db";

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
            List<string> categories = new() { "Work", "Health", "Travel", "Personal", "Education", "Finance", "Hobbies", "Relationships", "Spirituality", "Fitness", "Self-Care", "Social Life", "Career Development", "Projects", "Creativity", "Home & Family", "Wellness", "Leisure", "Events", "Goals & Planning" };

            foreach (var category in categories)
            {
                await conn.InsertAsync(
                    new Category() { Name = category }
                );
            }

        }

        #endregion


        #region Journal Entry Seed
        if (await conn.Table<JournalEntry>().CountAsync() == 0)
        {
            var faker = new Bogus.Faker();
            var moodIds = (await conn.Table<Mood>().ToListAsync()).Select(m => m.Id).ToList();
            var categoryIds = (await conn.Table<Category>().ToListAsync()).Select(c => c.Id).ToList();
            var tagIds = (await conn.Table<Tag>().ToListAsync()).Select(t => t.Id).ToList();

            var entryFaker = new Bogus.Faker<JournalEntry>()
                .RuleFor(e => e.Id, f => Guid.NewGuid())
                .RuleFor(e => e.EntryDate, f => f.Date.Between(DateTime.Now.AddMonths(-3), DateTime.Now.AddDays(-1)))
                .RuleFor(e => e.Title, f => f.Lorem.Sentence(3, 5).TrimEnd('.'))
                .RuleFor(e => e.RichText, f => $"<p>{f.Lorem.Paragraphs(4, 30)}</p>")
                .RuleFor(e => e.PrimaryMood, f => f.PickRandom(moodIds))
                .RuleFor(e => e.Category, f => f.PickRandom(categoryIds))
                .RuleFor(e => e.CreatedAt, f => f.Date.Past(1))
                .RuleFor(e => e.UpdatedAt, (f, e) => e.CreatedAt);

            var entries = entryFaker.Generate(75);

            var final = entries.DistinctBy(e => e.EntryDate.Date).ToList();

            foreach (var entry in final)
            {
                await conn.InsertAsync(entry);

                var secondaryMoodCount = faker.Random.Int(0, 2);
                var secondaryMoods = faker.PickRandom(moodIds.Where(m => m != entry.PrimaryMood), secondaryMoodCount);
                foreach (var moodId in secondaryMoods)
                {
                    await conn.InsertAsync(new JournalEntryMood
                    {
                        JournalEntryId = entry.Id,
                        MoodId = moodId
                    });
                }

                var tagCount = faker.Random.Int(2, 5);
                var selectedTags = faker.PickRandom(tagIds, tagCount);
                foreach (var tagId in selectedTags)
                {
                    await conn.InsertAsync(new JournalEntryTag
                    {
                        JournalEntryId = entry.Id,
                        TagId = tagId
                    });
                }
            }
        }
        #endregion


    }
}
