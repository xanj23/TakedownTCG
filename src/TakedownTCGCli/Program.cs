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

            string connectionString = configuration.GetValue<string>("Persistence:ConnectionString") ?? string.Empty;
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                connectionString = DatabaseConnectionDefaults.ResolveDefaultConnectionString();
            }

            try
            {
                PostgreSqlDatabaseInitializer.EnsureSchema(connectionString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PostgreSQL is unavailable; account and favorite features may not work in this CLI run. {ex.Message}");
            }

            UserRepository userRepository = new UserRepository(connectionString);
            FavoriteRepository favoriteRepository = new FavoriteRepository(connectionString);
            AccountService accountService = new AccountService(userRepository);
            FavoriteService favoriteService = new FavoriteService(favoriteRepository, userRepository);
            UserAccountController userAccountController = new UserAccountController(accountService, favoriteService);
            JustTcgHttpGateway httpGateway = new JustTcgHttpGateway();
            JustTcgResponseService responseService = new JustTcgResponseService();
            JustTcgQueryService queryService = new JustTcgQueryService();

            AppCompositionRoot.Configure(
                () => new JustTCGClient(apiOptions, httpGateway, queryService, responseService, userAccountController.OfferToFavorite),
                userAccountController);

            AppCompositionRoot.Run();
        }
    }
}
