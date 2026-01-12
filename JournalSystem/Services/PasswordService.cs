using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JournalSystem.Services
{
    public class PasswordService : IPasswordService
    {
        public async Task<string> GetKey()
        {
            return await SecureStorage.Default.GetAsync("password") ?? "";
        }

        public async Task<bool> HasKey()
        {
            string currpass = await GetKey();
            return currpass != "";
        }

        public async Task SetKey
            (string key)
        {
            string currpass = await GetKey();
            if(currpass != "")
            {
                throw new Exception("Password already set");
            }

            await SecureStorage.Default.SetAsync("password", key);
        }

        public async Task ValidatePassword(string password)
        {
            string currpass = await GetKey();
            if(currpass != password)
            {
                throw new Exception("Invalid password");
            }
        }
    }
}
