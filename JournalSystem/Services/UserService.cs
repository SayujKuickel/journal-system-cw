using JournalSystem.Services.Interface;

namespace JournalSystem.Services;

public class UserService : IUserService
{
    private readonly IPreferenceService preferenceService;
    private readonly IPasswordService passwordService;
    // constructor
    public UserService(IPreferenceService preferenceService, IPasswordService passwordService)
    {
        this.preferenceService = preferenceService;
        this.passwordService = passwordService;
    }

    // to create a register the user in the system
    public async Task CreateUser(string userName, string pin)
    {
        preferenceService.SetPreference(UserPreferences.Username, userName);
        await passwordService.SetKey(pin);
    }

    // Clears user data - pasword and preferences
    // Note: Doesn't remove any journal entries
    public void TerminateUser()
    {
        passwordService.TerminatePassword();
        preferenceService.TerminateAllPreferences();
    }
}
