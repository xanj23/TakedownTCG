using Npgsql;
using TakedownTCGApplication.Abstractions;
using TakedownTCGApplication.Models.UserAccounts;

namespace TakedownTCGApplication.Infrastructure.Persistence.UserAccounts;

public sealed class FavoriteRepository : IFavoriteRepository
{
    private readonly string _connectionString;

    public FavoriteRepository(string connectionString)
    {
        _connectionString = connectionString;
        PostgreSqlDatabaseInitializer.EnsureSchema(_connectionString);
    }

    public async Task<bool> AddFavoriteAsync(Favorite favorite)
    {
        await using NpgsqlConnection connection = new(_connectionString);
        await connection.OpenAsync();

        await using NpgsqlCommand command = new(
            """
            INSERT INTO favorites (user_name, item_type, item_id, item_name, created_at)
            VALUES (@UserName, @ItemType, @ItemId, @ItemName, @CreatedAt)
            ON CONFLICT (user_name, item_type, item_id) DO NOTHING;
            """,
            connection);

        command.Parameters.AddWithValue("@UserName", favorite.UserName);
        command.Parameters.AddWithValue("@ItemType", favorite.ItemType);
        command.Parameters.AddWithValue("@ItemId", favorite.ItemId);
        command.Parameters.AddWithValue("@ItemName", favorite.ItemName ?? string.Empty);
        command.Parameters.AddWithValue("@CreatedAt", favorite.CreatedAt);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> RemoveFavoriteAsync(string userName, string itemType, string itemId)
    {
        await using NpgsqlConnection connection = new(_connectionString);
        await connection.OpenAsync();

        await using NpgsqlCommand command = new(
            """
            DELETE FROM favorites
            WHERE user_name = @UserName AND item_type = @ItemType AND item_id = @ItemId;
            """,
            connection);

        command.Parameters.AddWithValue("@UserName", userName);
        command.Parameters.AddWithValue("@ItemType", itemType);
        command.Parameters.AddWithValue("@ItemId", itemId);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> FavoriteExistsAsync(string userName, string itemType, string itemId)
    {
        await using NpgsqlConnection connection = new(_connectionString);
        await connection.OpenAsync();

        await using NpgsqlCommand command = new(
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

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync();
    }

    public async Task<IReadOnlyList<Favorite>> GetFavoritesByUserAsync(string userName)
    {
        List<Favorite> results = new();

        await using NpgsqlConnection connection = new(_connectionString);
        await connection.OpenAsync();

        await using NpgsqlCommand command = new(
            """
            SELECT user_name, item_type, item_id, item_name, created_at
            FROM favorites
            WHERE user_name = @UserName
            ORDER BY created_at DESC;
            """,
            connection);

        command.Parameters.AddWithValue("@UserName", userName);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
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
