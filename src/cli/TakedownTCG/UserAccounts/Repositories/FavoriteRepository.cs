using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using TakedownTCG.UserAccounts.Models;

namespace TakedownTCG.UserAccounts.Repositories
{
    public class FavoriteRepository
    {
        private readonly string _connectionString;

        public FavoriteRepository(string databasePath)
        {
            _connectionString = $"Data Source={databasePath}";
            EnsureTableExists();
        }

        private void EnsureTableExists()
        {
            using SqliteConnection connection = new(_connectionString);
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Favorites (
                    UserName TEXT NOT NULL,
                    ItemType TEXT NOT NULL,
                    ItemId TEXT NOT NULL,
                    ItemName TEXT,
                    CreatedAt TEXT NOT NULL,
                    PRIMARY KEY (UserName, ItemType, ItemId)
                );
            ";
            command.ExecuteNonQuery();

            using SqliteCommand idx = connection.CreateCommand();
            idx.CommandText = @"
                CREATE INDEX IF NOT EXISTS idx_favorites_username ON Favorites (UserName);
            ";
            idx.ExecuteNonQuery();
        }

        public bool AddFavorite(Favorite fav)
        {
            using SqliteConnection connection = new(_connectionString);
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"
                INSERT OR IGNORE INTO Favorites (UserName, ItemType, ItemId, ItemName, CreatedAt)
                VALUES (@UserName, @ItemType, @ItemId, @ItemName, @CreatedAt);
            ";

            command.Parameters.AddWithValue("@UserName", fav.UserName);
            command.Parameters.AddWithValue("@ItemType", fav.ItemType);
            command.Parameters.AddWithValue("@ItemId", fav.ItemId);
            command.Parameters.AddWithValue("@ItemName", fav.ItemName ?? string.Empty);
            command.Parameters.AddWithValue("@CreatedAt", fav.CreatedAt.ToString("o"));

            return command.ExecuteNonQuery() > 0;
        }

        public bool RemoveFavorite(string userName, string itemType, string itemId)
        {
            using SqliteConnection connection = new(_connectionString);
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"
                DELETE FROM Favorites
                WHERE UserName = @UserName AND ItemType = @ItemType AND ItemId = @ItemId;
            ";

            command.Parameters.AddWithValue("@UserName", userName);
            command.Parameters.AddWithValue("@ItemType", itemType);
            command.Parameters.AddWithValue("@ItemId", itemId);

            return command.ExecuteNonQuery() > 0;
        }

        public bool FavoriteExists(string userName, string itemType, string itemId)
        {
            using SqliteConnection connection = new(_connectionString);
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"
                SELECT 1 FROM Favorites
                WHERE UserName = @UserName AND ItemType = @ItemType AND ItemId = @ItemId
                LIMIT 1;
            ";

            command.Parameters.AddWithValue("@UserName", userName);
            command.Parameters.AddWithValue("@ItemType", itemType);
            command.Parameters.AddWithValue("@ItemId", itemId);

            using SqliteDataReader reader = command.ExecuteReader();
            return reader.Read();
        }

        public List<Favorite> GetFavoritesByUser(string userName)
        {
            var results = new List<Favorite>();
            using SqliteConnection connection = new(_connectionString);
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"
                SELECT UserName, ItemType, ItemId, ItemName, CreatedAt
                FROM Favorites
                WHERE UserName = @UserName
                ORDER BY CreatedAt DESC;
            ";

            command.Parameters.AddWithValue("@UserName", userName);

            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                results.Add(new Favorite
                {
                    UserName = reader.GetString(0),
                    ItemType = reader.GetString(1),
                    ItemId = reader.GetString(2),
                    ItemName = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    CreatedAt = DateTime.Parse(reader.GetString(4), null, System.Globalization.DateTimeStyles.RoundtripKind)
                });
            }

            return results;
        }
    }
}
