using JournalSystem.Services.Interface;

namespace JournalSystem.Services
{
    public enum UserPreferences
    {
        Username,
        Theme
    }

    internal class PreferenceService : IPreferenceService
    {

        public void SetPreference(UserPreferences preference, string value)
        {

            Preferences.Set(preference.ToString(), value);
        }
        public string GetPreference(UserPreferences preference)
        {
            return Preferences.Get(preference.ToString(), null) ?? String.Empty;
        }

        public string GetUsername()
        {
            return GetPreference(UserPreferences.Username);
        }

        public void TerminatePreference(UserPreferences preferences)
        {
            Preferences.Remove(preferences.ToString());
        }

        public void TerminateAllPreferences()
        {
            foreach (UserPreferences pref in Enum.GetValues(typeof(UserPreferences)))
            {
                Preferences.Remove(pref.ToString());
            }
        }
    }
}
