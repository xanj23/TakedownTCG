using Microsoft.AspNetCore.Authentication.Cookies;
using TakedownTCG.Core.Abstractions;
using TakedownTCG.Core.Infrastructure.Config;
using TakedownTCG.Core.Infrastructure.Http;
using TakedownTCG.Core.Infrastructure.Persistence.UserAccounts;
using TakedownTCG.Core.Services.JustTcg;
using TakedownTCG.Core.Services.UserAccounts;

var builder = WebApplication.CreateBuilder(args);

JustTcgApiOptions apiOptions = builder.Configuration.GetSection("JustTcgApi").Get<JustTcgApiOptions>() ?? new JustTcgApiOptions();
string? apiKeyOverride = Environment.GetEnvironmentVariable("JUSTTCG_API_KEY");
if (!string.IsNullOrWhiteSpace(apiKeyOverride))
{
    apiOptions.ApiKey = apiKeyOverride;
}

PersistenceOptions persistenceOptions = builder.Configuration.GetSection("Persistence").Get<PersistenceOptions>() ?? new PersistenceOptions();
string? dbPathOverride = Environment.GetEnvironmentVariable("TAKEDOWNTCG_DB_PATH");
if (!string.IsNullOrWhiteSpace(dbPathOverride))
{
    persistenceOptions.DatabasePath = dbPathOverride;
}

if (string.IsNullOrWhiteSpace(persistenceOptions.DatabasePath))
{
    persistenceOptions.DatabasePath = DatabasePathDefaults.ResolveDefaultPath();
}

builder.Services.AddSingleton(apiOptions);
builder.Services.AddSingleton(persistenceOptions);

builder.Services.AddSingleton<IUserRepository>(sp =>
{
    PersistenceOptions options = sp.GetRequiredService<PersistenceOptions>();
    return new UserRepository(options.DatabasePath);
});

builder.Services.AddSingleton<IFavoriteRepository>(sp =>
{
    PersistenceOptions options = sp.GetRequiredService<PersistenceOptions>();
    return new FavoriteRepository(options.DatabasePath);
});

builder.Services.AddSingleton<IAccountService, AccountService>();
builder.Services.AddSingleton<IFavoriteService, FavoriteService>();
builder.Services.AddSingleton<IJustTcgHttpGateway, JustTcgHttpGateway>();
builder.Services.AddSingleton<JustTcgQueryService>();
builder.Services.AddSingleton<JustTcgResponseService>();
builder.Services.AddSingleton<IJustTcgResponseMapper>(sp => sp.GetRequiredService<JustTcgResponseService>());
builder.Services.AddSingleton<IJustTcgSearchService, JustTcgSearchService>();

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
