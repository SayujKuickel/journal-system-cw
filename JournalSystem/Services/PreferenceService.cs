using JournalSystem.Services.Interface;

namespace JournalSystem.Services
{
    // available preferences
    public enum UserPreferences
    {
        Username,
        Theme
    }

    internal class PreferenceService : IPreferenceService
    {
        // set preference.
        public void SetPreference(UserPreferences preference, string value)
        {

            Preferences.Set(preference.ToString(), value);
        }
        // get singl pereference.
        public string GetPreference(UserPreferences preference)
        {
            return Preferences.Get(preference.ToString(), null) ?? String.Empty;
        }
        // get username
        public string GetUsername()
        {
            return GetPreference(UserPreferences.Username);
        }
        // terminate single preference
        public void TerminatePreference(UserPreferences preferences)
        {
            Preferences.Remove(preferences.ToString());
        }
        // clear all preferences
        public void TerminateAllPreferences()
        {
            foreach (UserPreferences pref in Enum.GetValues(typeof(UserPreferences)))
            {
                Preferences.Remove(pref.ToString());
            }
        }
    }
}
