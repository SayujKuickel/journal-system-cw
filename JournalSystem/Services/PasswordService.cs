using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JournalSystem.Services
{
    public class PasswordService : IPasswordService
    {

        private const string PASSKEY = "user_password";

        // retrieves the stored key
        public async Task<string> GetKey()
        {
            return await SecureStorage.Default.GetAsync(PASSKEY) ?? "";
        }
        // checks if user has a key set.
        public async Task<bool> HasKey()
        {
            string currpass = await GetKey();
            return currpass != "";
        }
        // setting key (iniitial)
        public async Task SetKey(string key)
        {
            string currpass = await GetKey();
            if (currpass != "")
            {
                throw new Exception("Password already set");
            }

            await SecureStorage.Default.SetAsync(PASSKEY, key);
        }

        // service to change existing password
        public async Task ChangeKey(string oldKey, string newKey)
        {
            var currpass = await GetKey();
            if (currpass == "")
            {
                throw new Exception("Password not set");
            }

            if (currpass != oldKey)
            {
                throw new Exception("Invalid password");
            }

            if (string.IsNullOrWhiteSpace(newKey))
            {
                throw new Exception("New password is required");
            }

            SecureStorage.Default.Remove(PASSKEY);
            await SecureStorage.Default.SetAsync(PASSKEY, newKey);
        }

        // check user password against stored password
        public async Task ValidatePassword(string password)
        {
            string currpass = await GetKey();
            if (currpass != password)
            {
                throw new Exception("Invalid password");
            }
        }

        public void TerminatePassword()
        {
            SecureStorage.Default.Remove(PASSKEY);
        }
    }
}
