using JournalSystem.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JournalSystem.Services
{
    internal class PreferenceService: IPreferenceService
    {
        private const string key = "username";

        public void SetUsername(string username)
        {
            Preferences.Set(key, username);
        }

        public string GetUsername()
        {
            return Preferences.Get(key, null) ?? String.Empty;
        }
    }
}
