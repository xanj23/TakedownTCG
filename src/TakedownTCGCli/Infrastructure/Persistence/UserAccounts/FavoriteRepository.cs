using System;
using System.Collections.Generic;
using Npgsql;
using TakedownTCG.cli.Models.UserAccounts;

namespace TakedownTCG.cli.Infrastructure.Persistence.UserAccounts
{
    public class FavoriteRepository
    {
        private readonly string _connectionString;

        public FavoriteRepository(string connectionString)
        {
            _connectionString = connectionString;
            PostgreSqlDatabaseInitializer.EnsureSchema(_connectionString);
        }

        public bool AddFavorite(Favorite fav)
        {
            using NpgsqlConnection connection = new(_connectionString);
            connection.Open();

            using NpgsqlCommand command = new(
                """
                INSERT INTO favorites (user_name, item_type, item_id, item_name, created_at)
                VALUES (@UserName, @ItemType, @ItemId, @ItemName, @CreatedAt)
                ON CONFLICT (user_name, item_type, item_id) DO NOTHING;
                """,
                connection);

            command.Parameters.AddWithValue("@UserName", fav.UserName);
            command.Parameters.AddWithValue("@ItemType", fav.ItemType);
            command.Parameters.AddWithValue("@ItemId", fav.ItemId);
            command.Parameters.AddWithValue("@ItemName", fav.ItemName ?? string.Empty);
            command.Parameters.AddWithValue("@CreatedAt", fav.CreatedAt);

            return command.ExecuteNonQuery() > 0;
        }

        public bool RemoveFavorite(string userName, string itemType, string itemId)
        {
            using NpgsqlConnection connection = new(_connectionString);
            connection.Open();

            using NpgsqlCommand command = new(
                """
                DELETE FROM favorites
                WHERE user_name = @UserName AND item_type = @ItemType AND item_id = @ItemId;
                """,
                connection);

            command.Parameters.AddWithValue("@UserName", userName);
            command.Parameters.AddWithValue("@ItemType", itemType);
            command.Parameters.AddWithValue("@ItemId", itemId);

            return command.ExecuteNonQuery() > 0;
        }

        public bool FavoriteExists(string userName, string itemType, string itemId)
        {
            using NpgsqlConnection connection = new(_connectionString);
            connection.Open();

            using NpgsqlCommand command = new(
                """
                SELECT 1
                FROM favorites
                WHERE user_name = @UserName AND item_type = @ItemType AND item_id = @ItemId
                LIMIT 1;
                """,
                connection);

            command.Parameters.AddWithValue("@UserName", userName);
            command.Parameters.AddWithValue("@ItemType", itemType);
            command.Parameters.AddWithValue("@ItemId", itemId);

            using NpgsqlDataReader reader = command.ExecuteReader();
            return reader.Read();
        }

        public List<Favorite> GetFavoritesByUser(string userName)
        {
            List<Favorite> results = new();

            using NpgsqlConnection connection = new(_connectionString);
            connection.Open();

            using NpgsqlCommand command = new(
                """
                SELECT user_name, item_type, item_id, item_name, created_at
                FROM favorites
                WHERE user_name = @UserName
                ORDER BY created_at DESC;
                """,
                connection);

            command.Parameters.AddWithValue("@UserName", userName);

            using NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                results.Add(new Favorite
                {
                    UserName = reader.GetString(0),
                    ItemType = reader.GetString(1),
                    ItemId = reader.GetString(2),
                    ItemName = reader.GetString(3),
                    CreatedAt = reader.GetFieldValue<DateTime>(4)
                });
            }

            return results;
        }
    }
}
