using Microsoft.Data.Sqlite;
using TakedownTCGApplication.Abstractions;
using TakedownTCGApplication.Models.UserAccounts;

namespace TakedownTCGApplication.Infrastructure.Persistence.UserAccounts;

public sealed class FavoriteRepository : IFavoriteRepository
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

    public async Task<bool> AddFavoriteAsync(Favorite favorite)
    {
        await using SqliteConnection connection = new(_connectionString);
        await connection.OpenAsync();

        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT OR IGNORE INTO Favorites (UserName, ItemType, ItemId, ItemName, CreatedAt)
            VALUES (@UserName, @ItemType, @ItemId, @ItemName, @CreatedAt);
        ";

        command.Parameters.AddWithValue("@UserName", favorite.UserName);
        command.Parameters.AddWithValue("@ItemType", favorite.ItemType);
        command.Parameters.AddWithValue("@ItemId", favorite.ItemId);
        command.Parameters.AddWithValue("@ItemName", favorite.ItemName ?? string.Empty);
        command.Parameters.AddWithValue("@CreatedAt", favorite.CreatedAt.ToString("o"));

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> RemoveFavoriteAsync(string userName, string itemType, string itemId)
    {
        await using SqliteConnection connection = new(_connectionString);
        await connection.OpenAsync();

        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"
            DELETE FROM Favorites
            WHERE UserName = @UserName AND ItemType = @ItemType AND ItemId = @ItemId;
        ";

        command.Parameters.AddWithValue("@UserName", userName);
        command.Parameters.AddWithValue("@ItemType", itemType);
        command.Parameters.AddWithValue("@ItemId", itemId);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> FavoriteExistsAsync(string userName, string itemType, string itemId)
    {
        await using SqliteConnection connection = new(_connectionString);
        await connection.OpenAsync();

        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT 1 FROM Favorites
            WHERE UserName = @UserName AND ItemType = @ItemType AND ItemId = @ItemId
            LIMIT 1;
        ";

        command.Parameters.AddWithValue("@UserName", userName);
        command.Parameters.AddWithValue("@ItemType", itemType);
        command.Parameters.AddWithValue("@ItemId", itemId);

        await using SqliteDataReader reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync();
    }

    public async Task<IReadOnlyList<Favorite>> GetFavoritesByUserAsync(string userName)
    {
        List<Favorite> results = new();

        await using SqliteConnection connection = new(_connectionString);
        await connection.OpenAsync();

        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT UserName, ItemType, ItemId, ItemName, CreatedAt
            FROM Favorites
            WHERE UserName = @UserName
            ORDER BY CreatedAt DESC;
        ";

        command.Parameters.AddWithValue("@UserName", userName);

        await using SqliteDataReader reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
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
