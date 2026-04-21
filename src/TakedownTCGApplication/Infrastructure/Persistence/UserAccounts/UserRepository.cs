using Microsoft.Data.Sqlite;
using TakedownTCG.Core.Abstractions;
using TakedownTCG.Core.Models.UserAccounts;

namespace TakedownTCG.Core.Infrastructure.Persistence.UserAccounts;

public sealed class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(string databasePath)
    {
        _connectionString = $"Data Source={databasePath}";
        EnsureUserTableExists();
    }

    private void EnsureUserTableExists()
    {
        using SqliteConnection connection = new(_connectionString);
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS User (
                UserName TEXT PRIMARY KEY,
                UserEmail TEXT NOT NULL,
                PasswordHash TEXT NOT NULL,
                UserNotifications INTEGER NOT NULL DEFAULT 1
            );
        ";
        command.ExecuteNonQuery();
    }

    public async Task<bool> InsertUserAsync(User user)
    {
        await using SqliteConnection connection = new(_connectionString);
        await connection.OpenAsync();

        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO User (UserName, UserEmail, PasswordHash, UserNotifications)
            VALUES (@UserName, @UserEmail, @PasswordHash, @UserNotifications);
        ";

        command.Parameters.AddWithValue("@UserName", user.UserName);
        command.Parameters.AddWithValue("@UserEmail", user.UserEmail);
        command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
        command.Parameters.AddWithValue("@UserNotifications", user.UserNotifications ? 1 : 0);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<User?> GetByUserNameAsync(string userName)
    {
        await using SqliteConnection connection = new(_connectionString);
        await connection.OpenAsync();

        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT UserName, UserEmail, PasswordHash, UserNotifications
            FROM User
            WHERE UserName COLLATE NOCASE = @UserName
            LIMIT 1;
        ";
        command.Parameters.AddWithValue("@UserName", userName);

        await using SqliteDataReader reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new User
        {
            UserName = reader.GetString(0),
            UserEmail = reader.GetString(1),
            PasswordHash = reader.GetString(2),
            UserNotifications = reader.GetInt32(3) == 1
        };
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        await using SqliteConnection connection = new(_connectionString);
        await connection.OpenAsync();

        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT UserName, UserEmail, PasswordHash, UserNotifications
            FROM User
            WHERE UserEmail COLLATE NOCASE = @UserEmail
            LIMIT 1;
        ";
        command.Parameters.AddWithValue("@UserEmail", email);

        await using SqliteDataReader reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new User
        {
            UserName = reader.GetString(0),
            UserEmail = reader.GetString(1),
            PasswordHash = reader.GetString(2),
            UserNotifications = reader.GetInt32(3) == 1
        };
    }

    public async Task<User?> GetByUserNameOrEmailAsync(string input)
    {
        await using SqliteConnection connection = new(_connectionString);
        await connection.OpenAsync();

        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT UserName, UserEmail, PasswordHash, UserNotifications
            FROM User
            WHERE UserName COLLATE NOCASE = @Input
               OR UserEmail COLLATE NOCASE = @Input
            LIMIT 1;
        ";
        command.Parameters.AddWithValue("@Input", input);

        await using SqliteDataReader reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new User
        {
            UserName = reader.GetString(0),
            UserEmail = reader.GetString(1),
            PasswordHash = reader.GetString(2),
            UserNotifications = reader.GetInt32(3) == 1
        };
    }

    public async Task<bool> UpdateUserNameAsync(string currentUserName, string newUserName)
    {
        await using SqliteConnection connection = new(_connectionString);
        await connection.OpenAsync();

        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE User
            SET UserName = @NewUserName
            WHERE UserName COLLATE NOCASE = @CurrentUserName;
        ";
        command.Parameters.AddWithValue("@NewUserName", newUserName);
        command.Parameters.AddWithValue("@CurrentUserName", currentUserName);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> UpdateEmailAsync(string userName, string newEmail)
    {
        await using SqliteConnection connection = new(_connectionString);
        await connection.OpenAsync();

        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE User
            SET UserEmail = @NewEmail
            WHERE UserName COLLATE NOCASE = @UserName;
        ";
        command.Parameters.AddWithValue("@NewEmail", newEmail);
        command.Parameters.AddWithValue("@UserName", userName);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> UpdatePasswordHashAsync(string userName, string newPasswordHash)
    {
        await using SqliteConnection connection = new(_connectionString);
        await connection.OpenAsync();

        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE User
            SET PasswordHash = @PasswordHash
            WHERE UserName COLLATE NOCASE = @UserName;
        ";
        command.Parameters.AddWithValue("@PasswordHash", newPasswordHash);
        command.Parameters.AddWithValue("@UserName", userName);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> UpdateNotificationsAsync(string userName, bool enabled)
    {
        await using SqliteConnection connection = new(_connectionString);
        await connection.OpenAsync();

        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE User
            SET UserNotifications = @Enabled
            WHERE UserName COLLATE NOCASE = @UserName;
        ";
        command.Parameters.AddWithValue("@Enabled", enabled ? 1 : 0);
        command.Parameters.AddWithValue("@UserName", userName);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteUserAsync(string userName)
    {
        await using SqliteConnection connection = new(_connectionString);
        await connection.OpenAsync();

        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"
            DELETE FROM User
            WHERE UserName COLLATE NOCASE = @UserName;
        ";
        command.Parameters.AddWithValue("@UserName", userName);

        return await command.ExecuteNonQueryAsync() > 0;
    }
}
