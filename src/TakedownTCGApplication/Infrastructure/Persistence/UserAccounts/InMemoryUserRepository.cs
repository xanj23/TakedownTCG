using TakedownTCGApplication.Abstractions;
using TakedownTCGApplication.Models.UserAccounts;

namespace TakedownTCGApplication.Infrastructure.Persistence.UserAccounts;

public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly InMemoryAccountStore _store;

    public InMemoryUserRepository(InMemoryAccountStore store)
    {
        _store = store;
    }

    public Task<bool> InsertUserAsync(User user) => Task.FromResult(_store.InsertUser(user));
    public Task<User?> GetByUserNameAsync(string userName) => Task.FromResult(_store.GetByUserName(userName));
    public Task<User?> GetByEmailAsync(string email) => Task.FromResult(_store.GetByEmail(email));
    public Task<User?> GetByUserNameOrEmailAsync(string input) => Task.FromResult(_store.GetByUserNameOrEmail(input));
    public Task<bool> UpdateUserNameAsync(string currentUserName, string newUserName) => Task.FromResult(_store.UpdateUserName(currentUserName, newUserName));
    public Task<bool> UpdateEmailAsync(string userName, string newEmail) => Task.FromResult(_store.UpdateEmail(userName, newEmail));
    public Task<bool> UpdatePasswordHashAsync(string userName, string newPasswordHash) => Task.FromResult(_store.UpdatePasswordHash(userName, newPasswordHash));
    public Task<bool> UpdateNotificationsAsync(string userName, bool enabled) => Task.FromResult(_store.UpdateNotifications(userName, enabled));
    public Task<bool> DeleteUserAsync(string userName) => Task.FromResult(_store.DeleteUser(userName));
}
