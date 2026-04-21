namespace TakedownTCGApplication.Infrastructure.Config;

public sealed class PersistenceOptions
{
    public string ConnectionString { get; set; } = DatabaseConnectionDefaults.ResolveDefaultConnectionString();
}
