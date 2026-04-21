using TakedownTCGApplication.Abstractions;
using TakedownTCGApplication.Models.Ebay.Query;
using TakedownTCGApplication.Models.Ebay.Response;
using TakedownTCGApplication.Models.JustTcg.Query;
using TakedownTCGApplication.Models.JustTcg.Response;
using TakedownTCGApplication.Models.PokemonTcg.Query;
using TakedownTCGApplication.Models.SerpApi.Response;
using TakedownTCGApplication.Models.UserAccounts;
using TakedownTCGApplication.Infrastructure.Config;
using TakedownTCGApplication.Services.Ebay;
using TakedownTCGApplication.Services.JustTcg;
using TakedownTCGApplication.Services.PokemonTcg;
using TakedownTCGApplication.Services.SerpApi;
using TakedownTCGApplication.Services.UserAccounts;
using TakedownTCG.Tests.Shared;

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
    public void PokemonQueryBuilder_UsesRapidApiSearchShape()
    {
        PokemonTcgQueryService queryService = new();
        PokemonCardQueryParams query = new()
        {
            Search = "charizard ex 199",
            PerPage = 20,
            Page = 2,
            Sort = "price_highest"
        };

        string url = queryService.BuildCardsUrl(query, "https://pokemon-tcg-api.p.rapidapi.com");

        Assert.Equal(
            "https://pokemon-tcg-api.p.rapidapi.com/cards?per_page=20&page=2&sort=price_highest&search=charizard%20ex%20199",
            url);
    }

    [Fact]
    public void PokemonQueryBuilder_SupportsDirectTcgIdLookup()
    {
        PokemonTcgQueryService queryService = new();
        PokemonCardQueryParams query = new()
        {
            TcgId = "sv3-223",
            PerPage = 20,
            Page = 1,
            Sort = "price_highest"
        };

        string url = queryService.BuildCardsUrl(query, "https://pokemon-tcg-api.p.rapidapi.com");

        Assert.Equal(
            "https://pokemon-tcg-api.p.rapidapi.com/cards?per_page=20&page=1&sort=price_highest&tcgid=sv3-223",
            url);
    }

    [Fact]
    public async Task AccountService_FullLifecycle_Works()
    {
        InMemoryAccountStore store = new();
        IUserRepository userRepository = new InMemoryUserRepository(store);
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
        InMemoryAccountStore store = new();
        IUserRepository userRepository = new InMemoryUserRepository(store);
        IFavoriteRepository favoriteRepository = new InMemoryFavoriteRepository(store);
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

    [Fact]
    public void EbayQueryBuilder_UsesBrowseApiSearchShape()
    {
        EbayQueryService queryService = new();
        EbayItemSearchQueryParams query = new()
        {
            Search = "pokemon charizard psa 10",
            CategoryIds = "183454",
            BuyingOptions = "FIXED_PRICE|AUCTION",
            Sort = "newlyListed",
            Limit = 20,
            Offset = 40
        };

        string url = queryService.BuildItemSearchUrl(query, "https://api.ebay.com");

        Assert.Equal(
            "https://api.ebay.com/buy/browse/v1/item_summary/search?q=pokemon%20charizard%20psa%2010&limit=20&offset=40&category_ids=183454&filter=buyingOptions%3A%7BFIXED_PRICE%7CAUCTION%7D&sort=newlyListed",
            url);
    }

    [Fact]
    public void EbayMapper_MapsBrowseItemsToCardDisplayResults()
    {
        EbaySearchResultMapper mapper = new();
        EbayItemSummary item = new()
        {
            ItemId = "v1|123|0",
            Title = "Pokemon Charizard PSA 10",
            ItemWebUrl = "https://www.ebay.com/itm/123",
            Image = new EbayImage { ImageUrl = "https://i.ebayimg.com/images/123.jpg" },
            Price = new EbayMoney { Value = "249.99", Currency = "USD" },
            Condition = "Graded",
            BuyingOptions = ["FIXED_PRICE"],
            Seller = new EbaySeller { Username = "tcg-seller" },
            ItemCreationDate = "2026-04-20T12:00:00.000Z"
        };

        var results = mapper.MapItems([item], new HashSet<string>(StringComparer.OrdinalIgnoreCase));

        Assert.Single(results);
        Assert.Equal("Pokemon Charizard PSA 10", results[0].Name);
        Assert.Equal("eBay", results[0].Game);
        Assert.Equal("Active listing", results[0].SetName);
        Assert.Equal("USD", results[0].SetCode);
        Assert.Equal(249.99m, results[0].DisplayPrice);
        Assert.Equal("eBay listing price", results[0].DisplayPriceLabel);
        Assert.Equal("https://www.ebay.com/itm/123", results[0].TcgplayerProductUrl);
        Assert.Contains("Seller: tcg-seller", results[0].Details);
    }

    [Fact]
    public void SerpApiQueryBuilder_UsesSoldCompletedEbayCardsShape()
    {
        SerpApiQueryService queryService = new();
        SerpApiOptions options = new()
        {
            BaseUrl = "https://serpapi.com/search",
            EbayDomain = "ebay.com",
            DefaultQuery = "tcg trading card",
            CategoryId = "183454",
            ShowOnly = "Sold,Complete",
            PageSize = 25
        };

        string url = queryService.BuildCompletedSalesUrl(options);

        Assert.Equal(
            "https://serpapi.com/search?engine=ebay&ebay_domain=ebay.com&_nkw=tcg%20trading%20card&show_only=Sold%2CComplete&_ipg=25&category_id=183454",
            url);
    }

    [Fact]
    public void CompletedSaleMapper_LimitsCarouselItems()
    {
        CompletedTcgSaleMapper mapper = new();
        List<SerpApiEbayOrganicResult> results =
        [
            new()
            {
                Title = "Charizard PSA 10",
                ProductId = "1",
                Link = "https://www.ebay.com/itm/1",
                Thumbnail = "https://i.ebayimg.com/images/1.jpg",
                Condition = "Pre-Owned",
                Price = new SerpApiEbayPrice { Raw = "$120.00", Extracted = 120m },
                Seller = new SerpApiEbaySeller { Username = "seller-one" }
            },
            new()
            {
                Title = "Blue-Eyes White Dragon",
                ProductId = "2",
                Price = new SerpApiEbayPrice { Raw = "$80.00", Extracted = 80m }
            }
        ];

        var mapped = mapper.MapSales(results, 1);

        Assert.Single(mapped);
        Assert.Equal("Charizard PSA 10", mapped[0].Title);
        Assert.Equal("$120.00", mapped[0].PriceText);
        Assert.Equal(120m, mapped[0].Price);
        Assert.Equal("seller-one", mapped[0].Seller);
    }

    [Fact]
    public void PokemonResponseService_DeserializesAndMapsCards()
    {
        const string json = """
        {
          "data": [
            {
              "id": "base1-4",
              "name": "Charizard",
              "set": { "id": "base1", "name": "Base" },
              "number": "4",
              "rarity": "Rare Holo",
              "types": [ "Fire" ],
              "artist": "Mitsuhiro Arita",
              "images": {
                "small": "https://images.pokemontcg.io/base1/4.png",
                "large": "https://images.pokemontcg.io/base1/4_hires.png"
              },
              "tcgplayer": {
                "url": "https://www.tcgplayer.com/product/42382",
                "prices": {
                  "holofoil": { "market": 399.99 }
                }
              }
            }
          ],
          "page": 1,
          "pageSize": 20,
          "count": 1,
          "totalCount": 1
        }
        """;

        PokemonTcgResponseService responseService = new();
        PokemonSearchResultMapper mapper = new();

        var response = responseService.DeserializeCards(json);
        var results = mapper.MapCards(response.Data, new HashSet<string>(StringComparer.OrdinalIgnoreCase));

        Assert.Single(results);
        Assert.Equal("Charizard", results[0].Name);
        Assert.Equal("pokemon", results[0].Game);
        Assert.Equal("Base", results[0].SetName);
        Assert.Equal(399.99m, results[0].DisplayPrice);
    }

    [Fact]
    public void PokemonMapper_PrefersGradedPricesOverRawPrices()
    {
        const string json = """
        {
          "data": [
            {
              "id": 3852,
              "name": "Giratina VSTAR",
              "card_number": "GG69",
              "number": "GG69",
              "rarity": "Rare Secret",
              "episode": {
                "name": "Crown Zenith",
                "code": "CRZ"
              },
              "prices": {
                "cardmarket": {
                  "currency": "EUR",
                  "lowest_near_mint": 157.21,
                  "30d_average": 192.79,
                  "7d_average": 189.26,
                  "graded": {
                    "psa": { "psa10": 279, "psa9": 184 },
                    "cgc": { "cgc10": 344 }
                  }
                },
                "tcg_player": {
                  "currency": "USD",
                  "market_price": 146.69,
                  "mid_price": 163.71
                }
              }
            }
          ],
          "page": 1,
          "pageSize": 20,
          "count": 1,
          "totalCount": 1
        }
        """;

        PokemonTcgResponseService responseService = new();
        PokemonSearchResultMapper mapper = new();

        var response = responseService.DeserializeCards(json);
        var results = mapper.MapCards(response.Data, new HashSet<string>(StringComparer.OrdinalIgnoreCase));

        Assert.Single(results);
        Assert.Equal("3852", results[0].Id);
        Assert.Equal("Crown Zenith", results[0].SetName);
        Assert.Equal("CRZ", results[0].SetCode);
        Assert.Equal("GG69", results[0].Number);
        Assert.Equal(405.92m, results[0].DisplayPrice);
        Assert.Equal("Graded price (CGC 10)", results[0].DisplayPriceLabel);
        Assert.DoesNotContain("Graded price:", results[0].Details);
        Assert.Contains("TCGPlayer market: $146.69", results[0].Details);
        Assert.Contains("Cardmarket NM: $185.51", results[0].Details);
        Assert.Equal(3, results[0].VariantsCount);
    }

    [Fact]
    public void PokemonMapper_DeserializesListedGradedPriceShapes()
    {
        const string json = """
        {
          "data": [
            {
              "id": 3852,
              "name": "Giratina VSTAR",
              "prices": {
                "cardmarket": {
                  "currency": "EUR",
                  "lowest_near_mint": 157.21,
                  "graded": [
                    { "company": "psa", "grade": "10", "listed_price": 279 },
                    { "company": "cgc", "grade": "10", "price": 344 }
                  ]
                }
              }
            }
          ],
          "page": 1,
          "pageSize": 20,
          "count": 1,
          "totalCount": 1
        }
        """;

        PokemonTcgResponseService responseService = new();
        PokemonSearchResultMapper mapper = new();

        var response = responseService.DeserializeCards(json);
        var results = mapper.MapCards(response.Data, new HashSet<string>(StringComparer.OrdinalIgnoreCase));

        Assert.Single(results);
        Assert.Equal(405.92m, results[0].DisplayPrice);
        Assert.DoesNotContain("Graded price:", results[0].Details);
        Assert.Contains("Cardmarket NM: $185.51", results[0].Details);
        Assert.Equal(2, results[0].VariantsCount);
    }

    [Fact]
    public void PokemonResponseService_DeserializesNumericCardIds()
    {
        const string json = """
        {
          "data": [
            {
              "id": 3852,
              "name": "Pikachu"
            }
          ],
          "page": 1,
          "pageSize": 20,
          "count": 1,
          "totalCount": 1
        }
        """;

        PokemonTcgResponseService responseService = new();

        var response = responseService.DeserializeCards(json);

        Assert.Single(response.Data);
        Assert.Equal("3852", response.Data[0].Id);
    }

    [Fact]
    public void PokemonResponseService_DeserializesObjectValuesForStringFields()
    {
        const string json = """
        {
          "data": [
            {
              "id": "card_1",
              "name": "Bulbasaur",
              "artist": { "name": "Ken Sugimori" },
              "image": { "small": "https://example.test/bulbasaur.png" },
              "types": [
                { "name": "Grass" },
                "Poison"
              ]
            }
          ],
          "page": 1,
          "pageSize": 20,
          "count": 1,
          "totalCount": 1
        }
        """;

        PokemonTcgResponseService responseService = new();

        var response = responseService.DeserializeCards(json);

        Assert.Single(response.Data);
        Assert.Equal("Ken Sugimori", response.Data[0].Artist);
        Assert.Equal("https://example.test/bulbasaur.png", response.Data[0].Image);
        Assert.Equal(new[] { "Grass", "Poison" }, response.Data[0].Types);
    }

}
