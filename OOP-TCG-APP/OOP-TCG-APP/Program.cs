try
{
    FetchApi.SetApiHeader();
    await QueryMenu.Run();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
