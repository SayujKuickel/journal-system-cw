namespace JournalSystem.Services
{
    public interface IPasswordService
    {
        Task<string> GetKey();
        Task<bool> HasKey();
        Task SetKey(string key);
        Task ValidatePassword(string password);
        public bool TerminatePassword();
    }
}
