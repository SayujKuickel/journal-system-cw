namespace JournalSystem.Services;

public interface IPasswordService
{
    Task<string> GetKey();
    Task<bool> HasKey();
    Task SetKey(string key);
    Task ChangeKey(string oldKey, string newKey);
    Task ValidatePassword(string password);
    public void TerminatePassword();
}

