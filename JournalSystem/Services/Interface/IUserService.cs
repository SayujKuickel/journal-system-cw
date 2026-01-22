namespace JournalSystem.Services.Interface
{
    public interface IUserService
    {
        public Task CreateUser(string userName, string pin);
        public void TerminateUser();
    }

}
