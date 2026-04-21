namespace TakedownTCGApplication.Infrastructure.Config;

public static class DatabaseConnectionDefaults
{
    private const string DefaultConnectionString =
        "Host=localhost;Port=5432;Database=takedowntcg;Username=postgres;Password=postgres";

    public static string ResolveDefaultConnectionString()
    {
        return DefaultConnectionString;
    }
}
