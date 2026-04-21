namespace TakedownTCGApplication.Infrastructure.Config;

public sealed class PersistenceOptions
{
    public string DatabasePath { get; set; } = DatabasePathDefaults.ResolveDefaultPath();
}
