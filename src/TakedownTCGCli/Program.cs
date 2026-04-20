using TakedownTCG.cli.Composition;
using TakedownTCG.cli.Controllers;
using TakedownTCG.cli.Services.JustTcg;
using TakedownTCG.Core.Abstractions;
using TakedownTCG.Core.Infrastructure.Config;
using TakedownTCG.Core.Infrastructure.Http;
using TakedownTCG.Core.Infrastructure.Persistence.UserAccounts;
using TakedownTCG.Core.Services.JustTcg;
using TakedownTCG.Core.Services.UserAccounts;
using Microsoft.Extensions.Configuration;
using CoreJustTcgQueryService = TakedownTCG.Core.Services.JustTcg.JustTcgQueryService;
using CoreJustTcgResponseService = TakedownTCG.Core.Services.JustTcg.JustTcgResponseService;

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

            JustTcgApiOptions apiOptions = configuration.GetSection("JustTcgApi").Get<JustTcgApiOptions>() ?? new JustTcgApiOptions();
            string? apiKeyOverride = Environment.GetEnvironmentVariable("JUSTTCG_API_KEY");
            if (!string.IsNullOrWhiteSpace(apiKeyOverride))
            {
                apiOptions.ApiKey = apiKeyOverride;
            }

            PersistenceOptions persistenceOptions = configuration.GetSection("Persistence").Get<PersistenceOptions>() ?? new PersistenceOptions();
            string? dbPathOverride = Environment.GetEnvironmentVariable("TAKEDOWNTCG_DB_PATH");
            if (!string.IsNullOrWhiteSpace(dbPathOverride))
            {
                persistenceOptions.DatabasePath = dbPathOverride;
            }

            IUserRepository userRepository = new UserRepository(persistenceOptions.DatabasePath);
            IFavoriteRepository favoriteRepository = new FavoriteRepository(persistenceOptions.DatabasePath);
            IAccountService accountService = new AccountService(userRepository);
            IFavoriteService favoriteService = new FavoriteService(favoriteRepository, userRepository);
            IJustTcgHttpGateway httpGateway = new JustTcgHttpGateway();
            CoreJustTcgResponseService responseService = new CoreJustTcgResponseService();
            CoreJustTcgQueryService queryService = new CoreJustTcgQueryService();
            IJustTcgSearchService searchService = new JustTcgSearchService(apiOptions, queryService, responseService, httpGateway);

            UserAccountController.Configure(accountService, favoriteService);
            AppCompositionRoot.Configure(() => new JustTCGClient(apiOptions, searchService, responseService));

            AppCompositionRoot.Run();
        }
    }
}
