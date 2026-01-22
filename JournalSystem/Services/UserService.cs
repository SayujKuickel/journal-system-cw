using JournalSystem.Services.Interface;

namespace JournalSystem.Services;

public class UserService : IUserService
{
    private readonly IPreferenceService preferenceService;
    private readonly IPasswordService passwordService;

    public UserService(IPreferenceService preferenceService, IPasswordService passwordService)
    {
        this.preferenceService = preferenceService;
        this.passwordService = passwordService;
    }

    public async Task CreateUser(string userName, string pin)
    {
        preferenceService.SetPreference(UserPreferences.Username, userName);
        await passwordService.SetKey(pin);
    }

    public void TerminateUser()
    {
        passwordService.TerminatePassword();
        preferenceService.TerminateAllPreferences();
    }
}
