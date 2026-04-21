using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
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

PersistenceOptions persistenceOptions = builder.Configuration.GetSection("Persistence").Get<PersistenceOptions>() ?? new PersistenceOptions();
if (string.IsNullOrWhiteSpace(persistenceOptions.ConnectionString))
{
    persistenceOptions.ConnectionString = DatabaseConnectionDefaults.ResolveDefaultConnectionString();
}

bool usePostgresPersistence = true;
try
{
    PostgreSqlDatabaseInitializer.EnsureSchema(persistenceOptions.ConnectionString);
}
catch (Exception ex)
{
    usePostgresPersistence = false;
    Console.WriteLine($"PostgreSQL is unavailable; using in-memory accounts and favorites for this run. {ex.Message}");
}

PokemonTcgApiOptions pokemonApiOptions = builder.Configuration.GetSection("PokemonTcgApi").Get<PokemonTcgApiOptions>() ?? new PokemonTcgApiOptions();

EbayApiOptions ebayApiOptions = builder.Configuration.GetSection("EbayApi").Get<EbayApiOptions>() ?? new EbayApiOptions();
SerpApiOptions serpApiOptions = builder.Configuration.GetSection("SerpApi").Get<SerpApiOptions>() ?? new SerpApiOptions();

builder.Services.AddSingleton(apiOptions);
builder.Services.AddSingleton(persistenceOptions);
builder.Services.AddSingleton(pokemonApiOptions);
builder.Services.AddSingleton(ebayApiOptions);
builder.Services.AddSingleton(serpApiOptions);

if (usePostgresPersistence)
{
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
}
else
{
    builder.Services.AddSingleton<InMemoryAccountStore>();
    builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
    builder.Services.AddSingleton<IFavoriteRepository, InMemoryFavoriteRepository>();
}

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

DirectoryInfo dataProtectionKeysDirectory = Directory.CreateDirectory(
    Path.Combine(builder.Environment.ContentRootPath, "App_Data", "DataProtectionKeys"));

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(dataProtectionKeysDirectory)
    .SetApplicationName("TakedownTCGApplication");

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "TakedownTCG.Antiforgery.v2";
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.Cookie.Name = "TakedownTCG.Auth.v2";
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "TakedownTCG.Session.v2";
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
