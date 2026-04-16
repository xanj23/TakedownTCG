namespace TakedownTCG.cli.Views.Output
{
    /// <summary>
    /// Renders JustTCG output to the console.
    /// </summary>
    public static class JustTcgOutputView
    {
        public static void DisplayMappedData(string mappedData)
        {
            Console.WriteLine();
            Console.WriteLine(mappedData);
        }
    }
}
