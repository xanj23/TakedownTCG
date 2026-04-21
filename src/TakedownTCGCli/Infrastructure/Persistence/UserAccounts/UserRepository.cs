using Npgsql;
using TakedownTCG.cli.Models.UserAccounts;

namespace TakedownTCG.cli.Infrastructure.Persistence.UserAccounts
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
            PostgreSqlDatabaseInitializer.EnsureSchema(_connectionString);
        }

        public bool InsertUser(User user)
        {
            using NpgsqlConnection connection = new(_connectionString);
            connection.Open();

            using NpgsqlCommand command = new(
                """
                INSERT INTO users (user_name, user_email, password_hash, user_notifications)
                VALUES (@UserName, @UserEmail, @PasswordHash, @UserNotifications);
                """,
                connection);

            command.Parameters.AddWithValue("@UserName", user.UserName);
            command.Parameters.AddWithValue("@UserEmail", user.UserEmail);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@UserNotifications", user.UserNotifications);

            try
            {
                return command.ExecuteNonQuery() > 0;
            }
            catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                return false;
            }
        }

        public User? GetByUserName(string userName)
        {
            using NpgsqlConnection connection = new(_connectionString);
            connection.Open();

            using NpgsqlCommand command = new(
                """
                SELECT user_name, user_email, password_hash, user_notifications
                FROM users
                WHERE LOWER(user_name) = LOWER(@UserName)
                LIMIT 1;
                """,
                connection);

            command.Parameters.AddWithValue("@UserName", userName);

            using NpgsqlDataReader reader = command.ExecuteReader();
            return reader.Read() ? ReadUser(reader) : null;
        }

        public User? GetByEmail(string email)
        {
            using NpgsqlConnection connection = new(_connectionString);
            connection.Open();

            using NpgsqlCommand command = new(
                """
                SELECT user_name, user_email, password_hash, user_notifications
                FROM users
                WHERE LOWER(user_email) = LOWER(@UserEmail)
                LIMIT 1;
                """,
                connection);

            command.Parameters.AddWithValue("@UserEmail", email);

            using NpgsqlDataReader reader = command.ExecuteReader();
            return reader.Read() ? ReadUser(reader) : null;
        }

        public User? GetByUserNameOrEmail(string input)
        {
            using NpgsqlConnection connection = new(_connectionString);
            connection.Open();

            using NpgsqlCommand command = new(
                """
                SELECT user_name, user_email, password_hash, user_notifications
                FROM users
                WHERE LOWER(user_name) = LOWER(@Input)
                   OR LOWER(user_email) = LOWER(@Input)
                LIMIT 1;
                """,
                connection);

            command.Parameters.AddWithValue("@Input", input);

            using NpgsqlDataReader reader = command.ExecuteReader();
            return reader.Read() ? ReadUser(reader) : null;
        }

        public bool UpdateUserName(string currentUserName, string newUserName)
        {
            using NpgsqlConnection connection = new(_connectionString);
            connection.Open();

            using NpgsqlCommand command = new(
                """
                UPDATE users
                SET user_name = @NewUserName
                WHERE LOWER(user_name) = LOWER(@CurrentUserName);
                """,
                connection);

            command.Parameters.AddWithValue("@NewUserName", newUserName);
            command.Parameters.AddWithValue("@CurrentUserName", currentUserName);

            try
            {
                return command.ExecuteNonQuery() > 0;
            }
            catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                return false;
            }
        }

        public bool UpdateEmail(string userName, string newEmail)
        {
            using NpgsqlConnection connection = new(_connectionString);
            connection.Open();

            using NpgsqlCommand command = new(
                """
                UPDATE users
                SET user_email = @NewEmail
                WHERE LOWER(user_name) = LOWER(@UserName);
                """,
                connection);

            command.Parameters.AddWithValue("@NewEmail", newEmail);
            command.Parameters.AddWithValue("@UserName", userName);

            try
            {
                return command.ExecuteNonQuery() > 0;
            }
            catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                return false;
            }
        }

        public bool UpdatePasswordHash(string userName, string newPasswordHash)
        {
            using NpgsqlConnection connection = new(_connectionString);
            connection.Open();

            using NpgsqlCommand command = new(
                """
                UPDATE users
                SET password_hash = @PasswordHash
                WHERE LOWER(user_name) = LOWER(@UserName);
                """,
                connection);

            command.Parameters.AddWithValue("@PasswordHash", newPasswordHash);
            command.Parameters.AddWithValue("@UserName", userName);

            return command.ExecuteNonQuery() > 0;
        }

        public bool UpdateNotifications(string userName, bool enabled)
        {
            using NpgsqlConnection connection = new(_connectionString);
            connection.Open();

            using NpgsqlCommand command = new(
                """
                UPDATE users
                SET user_notifications = @Enabled
                WHERE LOWER(user_name) = LOWER(@UserName);
                """,
                connection);

            command.Parameters.AddWithValue("@Enabled", enabled);
            command.Parameters.AddWithValue("@UserName", userName);

            return command.ExecuteNonQuery() > 0;
        }

        public bool DeleteUser(string userName)
        {
            using NpgsqlConnection connection = new(_connectionString);
            connection.Open();

            using NpgsqlCommand command = new(
                """
                DELETE FROM users
                WHERE LOWER(user_name) = LOWER(@UserName);
                """,
                connection);

            command.Parameters.AddWithValue("@UserName", userName);

            return command.ExecuteNonQuery() > 0;
        }

        private static User ReadUser(NpgsqlDataReader reader)
        {
            return new User
            {
                UserName = reader.GetString(0),
                UserEmail = reader.GetString(1),
                PasswordHash = reader.GetString(2),
                UserNotifications = reader.GetBoolean(3)
            };
        }
    }
}
