namespace TakedownTCGApplication.Infrastructure.Config;

public static class DatabasePathDefaults
{
    public static string ResolveDefaultPath()
    {
        string root = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));

        string dataDirectory = Path.Combine(root, "data");
        Directory.CreateDirectory(dataDirectory);

        return Path.Combine(dataDirectory, "takedowntcg-users.db");
    }
}
