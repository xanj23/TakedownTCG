using TakedownTCG.Core.Abstractions;
using TakedownTCG.Core.Infrastructure.Persistence.UserAccounts;
using TakedownTCG.Core.Models.JustTcg.Query;
using TakedownTCG.Core.Models.JustTcg.Response;
using TakedownTCG.Core.Models.UserAccounts;
using TakedownTCG.Core.Services.JustTcg;
using TakedownTCG.Core.Services.UserAccounts;

namespace TakedownTCG.Core.Tests;

public sealed class CoreBehaviorTests
{
    [Fact]
    public void QueryBuilder_EncodesParameters()
    {
        JustTcgQueryService queryService = new();
        CardQueryParams query = new();
        query.Parameters["q"].Value = "Black Lotus";
        query.Parameters["printing"].Value = "1st edition";

        string url = queryService.BuildUrl(JustTcgEndpoint.Cards, query, "https://api.justtcg.com/v1");

        Assert.Equal("https://api.justtcg.com/v1/cards?q=Black%20Lotus&printing=1st%20edition", url);
    }

    [Fact]
    public async Task AccountService_FullLifecycle_Works()
    {
        await using TestDatabase db = new();
        IUserRepository userRepository = new UserRepository(db.Path);
        IAccountService accountService = new AccountService(userRepository);

        bool created = await accountService.CreateAccountAsync("tester", "tester@example.com", "Password123!");
        Assert.True(created);

        User? loggedIn = await accountService.LoginAsync("tester", "Password123!");
        Assert.NotNull(loggedIn);

        bool changedName = await accountService.ChangeUserNameAsync("tester", "tester2");
        Assert.True(changedName);

        bool changedEmail = await accountService.ChangeEmailAsync("tester2", "tester2@example.com");
        Assert.True(changedEmail);

        bool changedPassword = await accountService.ChangePasswordAsync("tester2", "Password123!", "Password456!");
        Assert.True(changedPassword);

        User? loginAfterPasswordChange = await accountService.LoginAsync("tester2", "Password456!");
        Assert.NotNull(loginAfterPasswordChange);

        bool notificationsUpdated = await accountService.UpdateUserNotificationsAsync("tester2", false);
        Assert.True(notificationsUpdated);

        bool deleted = await accountService.DeleteAccountAsync("tester2");
        Assert.True(deleted);

        User? afterDelete = await accountService.LoginAsync("tester2", "Password456!");
        Assert.Null(afterDelete);
    }

    [Fact]
    public async Task FavoriteService_PreventsDuplicateFavorites()
    {
        await using TestDatabase db = new();
        IUserRepository userRepository = new UserRepository(db.Path);
        IFavoriteRepository favoriteRepository = new FavoriteRepository(db.Path);
        IAccountService accountService = new AccountService(userRepository);
        IFavoriteService favoriteService = new FavoriteService(favoriteRepository, userRepository);

        bool created = await accountService.CreateAccountAsync("favoriter", "favoriter@example.com", "Password123!");
        Assert.True(created);

        bool addedFirst = await favoriteService.AddFavoriteAsync("favoriter", "Card", "abc123", "Test Card");
        bool addedDuplicate = await favoriteService.AddFavoriteAsync("favoriter", "Card", "abc123", "Test Card");

        Assert.True(addedFirst);
        Assert.False(addedDuplicate);

        IReadOnlyList<Favorite> favorites = await favoriteService.GetFavoritesAsync("favoriter");
        Assert.Single(favorites);

        bool removed = await favoriteService.RemoveFavoriteAsync("favoriter", "Card", "abc123");
        Assert.True(removed);
    }

    [Fact]
    public void ResponseService_DeserializesAndMapsCards()
    {
        const string json = """
        {
          "data": [
            {
              "id": "card_1",
              "name": "Lightning Bolt",
              "game": "mtg",
              "set": "lea",
              "set_name": "Limited Edition Alpha",
              "number": "161",
              "tcgplayerId": "100",
              "mtgjsonId": "100",
              "scryfallId": "100",
              "rarity": "Common",
              "details": "Deal 3 damage",
              "variants": []
            }
          ],
          "meta": { "total": 1, "limit": 50, "offset": 0, "has_more": false },
          "_metadata": {
            "api_plan": "dev",
            "api_request_limit": 1000,
            "api_requests_used": 1,
            "api_requests_remaining": 999,
            "api_daily_limit": 500,
            "api_daily_requests_used": 1,
            "api_daily_requests_remaining": 499,
            "api_rate_limit": 60
          },
          "error": null,
          "code": null
        }
        """;

        JustTcgResponseService service = new();
        object result = service.Deserialize(JustTcgEndpoint.Cards, json);
        string mapped = service.Map(result);

        Assert.IsType<Response<Card>>(result);
        Assert.Contains("Lightning Bolt", mapped);
        Assert.Contains("Results: 1", mapped);
    }

    private sealed class TestDatabase : IAsyncDisposable
    {
        public string Path { get; }

        public TestDatabase()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"takedowntcg-core-tests-{Guid.NewGuid():N}.db");
        }

        public ValueTask DisposeAsync()
        {
            if (File.Exists(Path))
            {
                try
                {
                    File.Delete(Path);
                }
                catch (IOException)
                {
                    // Windows may keep the SQLite file handle briefly after test completion.
                }
            }

            return ValueTask.CompletedTask;
        }
    }
}
