using Microsoft.Data.Sqlite;
using TakedownTCG.cli.Models.UserAccounts;

namespace TakedownTCG.cli.Infrastructure.Persistence.UserAccounts
{
    public class UserRepository
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

        public bool InsertUser(User user)
        {
            using SqliteConnection connection = new(_connectionString);
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO User (UserName, UserEmail, PasswordHash, UserNotifications)
                VALUES (@UserName, @UserEmail, @PasswordHash, @UserNotifications);
            ";

            command.Parameters.AddWithValue("@UserName", user.UserName);
            command.Parameters.AddWithValue("@UserEmail", user.UserEmail);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@UserNotifications", user.UserNotifications ? 1 : 0);

            return command.ExecuteNonQuery() > 0;
        }

        public User? GetByUserName(string userName)
        {
            using SqliteConnection connection = new(_connectionString);
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"
                SELECT UserName, UserEmail, PasswordHash, UserNotifications
                FROM User
                WHERE UserName = @UserName
                LIMIT 1;
            ";
            command.Parameters.AddWithValue("@UserName", userName);

            using SqliteDataReader reader = command.ExecuteReader();
            if (!reader.Read())
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

        public User? GetByEmail(string email)
        {
            using SqliteConnection connection = new(_connectionString);
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"
                SELECT UserName, UserEmail, PasswordHash, UserNotifications
                FROM User
                WHERE UserEmail = @UserEmail
                LIMIT 1;
            ";
            command.Parameters.AddWithValue("@UserEmail", email);

            using SqliteDataReader reader = command.ExecuteReader();
            if (!reader.Read())
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

        public User? GetByUserNameOrEmail(string input)
        {
            using SqliteConnection connection = new(_connectionString);
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"
                SELECT UserName, UserEmail, PasswordHash, UserNotifications
                FROM User
                WHERE UserName = @Input OR UserEmail = @Input
                LIMIT 1;
            ";
            command.Parameters.AddWithValue("@Input", input);

            using SqliteDataReader reader = command.ExecuteReader();
            if (!reader.Read())
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

        public bool UpdateUserName(string currentUserName, string newUserName)
        {
            using SqliteConnection connection = new(_connectionString);
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE User
                SET UserName = @NewUserName
                WHERE UserName = @CurrentUserName;
            ";
            command.Parameters.AddWithValue("@NewUserName", newUserName);
            command.Parameters.AddWithValue("@CurrentUserName", currentUserName);

            return command.ExecuteNonQuery() > 0;
        }

        public bool UpdateEmail(string userName, string newEmail)
        {
            using SqliteConnection connection = new(_connectionString);
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE User
                SET UserEmail = @NewEmail
                WHERE UserName = @UserName;
            ";
            command.Parameters.AddWithValue("@NewEmail", newEmail);
            command.Parameters.AddWithValue("@UserName", userName);

            return command.ExecuteNonQuery() > 0;
        }

        public bool UpdatePasswordHash(string userName, string newPasswordHash)
        {
            using SqliteConnection connection = new(_connectionString);
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE User
                SET PasswordHash = @PasswordHash
                WHERE UserName = @UserName;
            ";
            command.Parameters.AddWithValue("@PasswordHash", newPasswordHash);
            command.Parameters.AddWithValue("@UserName", userName);

            return command.ExecuteNonQuery() > 0;
        }

        public bool UpdateNotifications(string userName, bool enabled)
        {
            using SqliteConnection connection = new(_connectionString);
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE User
                SET UserNotifications = @Enabled
                WHERE UserName = @UserName;
            ";
            command.Parameters.AddWithValue("@Enabled", enabled ? 1 : 0);
            command.Parameters.AddWithValue("@UserName", userName);

            return command.ExecuteNonQuery() > 0;
        }

        public bool DeleteUser(string userName)
        {
            using SqliteConnection connection = new(_connectionString);
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"
                DELETE FROM User
                WHERE UserName = @UserName;
            ";
            command.Parameters.AddWithValue("@UserName", userName);

            return command.ExecuteNonQuery() > 0;
        }
    }
}
