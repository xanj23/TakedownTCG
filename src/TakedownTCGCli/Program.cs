using TakedownTCG.cli.Composition;
using TakedownTCG.cli.Controllers;
using TakedownTCG.cli.Infrastructure.Config;
using TakedownTCG.cli.Infrastructure.Http;
using TakedownTCG.cli.Infrastructure.Persistence.UserAccounts;
using TakedownTCG.cli.Services.JustTcg;
using TakedownTCG.cli.Services.UserAccounts;
using Microsoft.Extensions.Configuration;

namespace TakedownTCG.cli
{
    public static class Program
    {
        public static void Main()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            JustTcgApiConfig apiOptions = configuration.GetSection("JustTcgApi").Get<JustTcgApiConfig>() ?? new JustTcgApiConfig();
            string? apiKeyOverride = Environment.GetEnvironmentVariable("JUSTTCG_API_KEY");
            if (!string.IsNullOrWhiteSpace(apiKeyOverride))
            {
                apiOptions.ApiKey = apiKeyOverride;
            }

            string databasePath = configuration.GetValue<string>("Persistence:DatabasePath") ?? string.Empty;
            string? dbPathOverride = Environment.GetEnvironmentVariable("TAKEDOWNTCG_DB_PATH");
            if (!string.IsNullOrWhiteSpace(dbPathOverride))
            {
                databasePath = dbPathOverride;
            }

            if (string.IsNullOrWhiteSpace(databasePath))
            {
                databasePath = Path.Combine(AppContext.BaseDirectory, "takedowntcg.db");
            }

            UserRepository userRepository = new UserRepository(databasePath);
            FavoriteRepository favoriteRepository = new FavoriteRepository(databasePath);
            AccountService accountService = new AccountService(userRepository);
            FavoriteService favoriteService = new FavoriteService(favoriteRepository, userRepository);
            JustTcgHttpGateway httpGateway = new JustTcgHttpGateway();
            JustTcgResponseService responseService = new JustTcgResponseService();
            JustTcgQueryService queryService = new JustTcgQueryService();

            UserAccountController.Configure(accountService, favoriteService);
            AppCompositionRoot.Configure(() => new JustTCGClient(apiOptions, httpGateway, queryService, responseService));

            AppCompositionRoot.Run();
        }
    }
}
