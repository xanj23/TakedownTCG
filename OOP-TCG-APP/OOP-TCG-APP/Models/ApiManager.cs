namespace TCGAPP
{
    public class ApiManager
    {
        public static readonly List<IApi> ApiRepositories = new List<IApi>();
        public static int NumofApis = 0;

        public static void RegisterApi(IApi api)
        {
            ApiRepositories.Add(api);
            NumofApis = ApiRepositories.Count;
        }
    }
}