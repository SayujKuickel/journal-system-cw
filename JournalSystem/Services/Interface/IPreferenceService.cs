using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JournalSystem.Services.Interface
{
    public interface IPreferenceService
    {
        public void SetUsername(string username);
        public  string GetUsername();
    }
}
