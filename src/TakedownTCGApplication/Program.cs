using Microsoft.AspNetCore.Authentication.Cookies;
using TakedownTCGApplication.Abstractions;
using TakedownTCGApplication.Infrastructure.Config;
using TakedownTCGApplication.Infrastructure.Http;
using TakedownTCGApplication.Services.Ebay;
using TakedownTCGApplication.Infrastructure.Persistence.UserAccounts;
using TakedownTCGApplication.Services.JustTcg;
using TakedownTCGApplication.Services.PokemonTcg;
using TakedownTCGApplication.Services.SerpApi;
using TakedownTCGApplication.Services.UserAccounts;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

JustTcgApiOptions apiOptions = builder.Configuration.GetSection("JustTcgApi").Get<JustTcgApiOptions>() ?? new JustTcgApiOptions();
string? apiKeyOverride = Environment.GetEnvironmentVariable("JUSTTCG_API_KEY");
if (!string.IsNullOrWhiteSpace(apiKeyOverride))
{
    apiOptions.ApiKey = apiKeyOverride;
}

PersistenceOptions persistenceOptions = builder.Configuration.GetSection("Persistence").Get<PersistenceOptions>() ?? new PersistenceOptions();
string? dbConnectionStringOverride = Environment.GetEnvironmentVariable("TAKEDOWNTCG_DB_CONNECTION_STRING");
if (!string.IsNullOrWhiteSpace(dbConnectionStringOverride))
{
    persistenceOptions.ConnectionString = dbConnectionStringOverride;
}

if (string.IsNullOrWhiteSpace(persistenceOptions.ConnectionString))
{
    persistenceOptions.ConnectionString = DatabaseConnectionDefaults.ResolveDefaultConnectionString();
}

PokemonTcgApiOptions pokemonApiOptions = builder.Configuration.GetSection("PokemonTcgApi").Get<PokemonTcgApiOptions>() ?? new PokemonTcgApiOptions();
string? pokemonApiKeyOverride = Environment.GetEnvironmentVariable("POKEMON_TCG_API_KEY")
                                ?? Environment.GetEnvironmentVariable("RAPIDAPI_KEY");
if (!string.IsNullOrWhiteSpace(pokemonApiKeyOverride))
{
    pokemonApiOptions.ApiKey = pokemonApiKeyOverride;
}

EbayApiOptions ebayApiOptions = builder.Configuration.GetSection("EbayApi").Get<EbayApiOptions>() ?? new EbayApiOptions();
SerpApiOptions serpApiOptions = builder.Configuration.GetSection("SerpApi").Get<SerpApiOptions>() ?? new SerpApiOptions();

builder.Services.AddSingleton(apiOptions);
builder.Services.AddSingleton(persistenceOptions);
builder.Services.AddSingleton(pokemonApiOptions);
builder.Services.AddSingleton(ebayApiOptions);
builder.Services.AddSingleton(serpApiOptions);

builder.Services.AddSingleton<IUserRepository>(sp =>
{
    PersistenceOptions options = sp.GetRequiredService<PersistenceOptions>();
    return new UserRepository(options.ConnectionString);
});

builder.Services.AddSingleton<IFavoriteRepository>(sp =>
{
    PersistenceOptions options = sp.GetRequiredService<PersistenceOptions>();
    return new FavoriteRepository(options.ConnectionString);
});

builder.Services.AddSingleton<IAccountService, AccountService>();
builder.Services.AddSingleton<IFavoriteService, FavoriteService>();
builder.Services.AddSingleton<IJustTcgHttpGateway, JustTcgHttpGateway>();
builder.Services.AddSingleton<IPokemonTcgHttpGateway, PokemonTcgHttpGateway>();
builder.Services.AddSingleton<IEbayHttpGateway, EbayHttpGateway>();
builder.Services.AddSingleton<ISerpApiHttpGateway, SerpApiHttpGateway>();
builder.Services.AddSingleton<JustTcgQueryService>();
builder.Services.AddSingleton<JustTcgResponseService>();
builder.Services.AddSingleton<PokemonTcgQueryService>();
builder.Services.AddSingleton<PokemonTcgResponseService>();
builder.Services.AddSingleton<EbayQueryService>();
builder.Services.AddSingleton<EbayResponseService>();
builder.Services.AddSingleton<EbayOAuthTokenService>();
builder.Services.AddSingleton<SerpApiQueryService>();
builder.Services.AddSingleton<SerpApiResponseService>();
builder.Services.AddSingleton<IJustTcgResponseMapper>(sp => sp.GetRequiredService<JustTcgResponseService>());
builder.Services.AddSingleton<IJustTcgSearchService, JustTcgSearchService>();
builder.Services.AddSingleton<IPokemonTcgSearchService, PokemonTcgSearchService>();
builder.Services.AddSingleton<IEbaySearchService, EbaySearchService>();
builder.Services.AddSingleton<IProductsSearchResultMapper, ProductsSearchResultMapper>();
builder.Services.AddSingleton<IPokemonSearchResultMapper, PokemonSearchResultMapper>();
builder.Services.AddSingleton<IEbaySearchResultMapper, EbaySearchResultMapper>();
builder.Services.AddSingleton<ICompletedTcgSaleMapper, CompletedTcgSaleMapper>();
builder.Services.AddSingleton<IProductsSearchService, ProductsSearchService>();
builder.Services.AddSingleton<IPokemonProductsSearchService, PokemonProductsSearchService>();
builder.Services.AddSingleton<IEbayProductsSearchService, EbayProductsSearchService>();
builder.Services.AddSingleton<IProductsSearchWorkflow, ProductsSearchWorkflow>();
builder.Services.AddSingleton<ICompletedTcgSalesService, CompletedTcgSalesService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.Cookie.Name = "TakedownTCG.Auth";
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "TakedownTCG.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromHours(8);
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

public partial class Program
{
}
