using Npgsql;

namespace TakedownTCG.cli.Infrastructure.Persistence.UserAccounts
{
    internal static class PostgreSqlDatabaseInitializer
    {
        private const string SchemaSql = """
            CREATE TABLE IF NOT EXISTS users (
                user_name TEXT NOT NULL,
                user_email TEXT NOT NULL,
                password_hash TEXT NOT NULL,
                user_notifications BOOLEAN NOT NULL DEFAULT TRUE,
                CONSTRAINT pk_users PRIMARY KEY (user_name)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS ux_users_lower_user_name
                ON users (LOWER(user_name));

            CREATE UNIQUE INDEX IF NOT EXISTS ux_users_lower_user_email
                ON users (LOWER(user_email));

            CREATE TABLE IF NOT EXISTS favorites (
                user_name TEXT NOT NULL,
                item_type TEXT NOT NULL,
                item_id TEXT NOT NULL,
                item_name TEXT NOT NULL DEFAULT '',
                created_at TIMESTAMPTZ NOT NULL,
                CONSTRAINT pk_favorites PRIMARY KEY (user_name, item_type, item_id),
                CONSTRAINT fk_favorites_users FOREIGN KEY (user_name)
                    REFERENCES users (user_name)
                    ON UPDATE CASCADE
                    ON DELETE CASCADE
            );

            CREATE INDEX IF NOT EXISTS ix_favorites_user_name_created_at
                ON favorites (user_name, created_at DESC);
            """;

        public static void EnsureSchema(string connectionString)
        {
            using NpgsqlConnection connection = new(connectionString);
            connection.Open();

            using NpgsqlCommand command = new(SchemaSql, connection);
            command.ExecuteNonQuery();
        }
    }
}
