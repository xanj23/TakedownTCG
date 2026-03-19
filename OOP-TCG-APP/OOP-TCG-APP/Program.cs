try
{
    FetchApi.SetApiHeader();
    await ApiHandler.Run();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
