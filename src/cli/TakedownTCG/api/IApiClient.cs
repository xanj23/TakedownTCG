namespace TakedownTCG.cli.Api
{
    public interface IApiClient
    {
        string Name { get; }
        string BaseUrl { get; }
        string ApiKey { get; }
        void Run();
    }
}
