using System;

namespace TCGAPP
{

    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                ApiManager.LoadApis();
                await ApiHandler.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message} [Program]");
            }
        }
    }
}
