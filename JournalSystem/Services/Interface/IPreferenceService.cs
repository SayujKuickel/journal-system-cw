using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JournalSystem.Services.Interface
{
    public interface IPreferenceService
    {
        public void SetPreference(UserPreferences preference, string value);
        public string GetPreference(UserPreferences preference);
        public void TerminatePreference(UserPreferences preferences);
        public void TerminateAllPreferences();
        public string GetUsername();
    }
}
