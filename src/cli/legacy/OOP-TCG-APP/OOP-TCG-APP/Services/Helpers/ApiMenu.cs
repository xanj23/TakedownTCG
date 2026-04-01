using System;
using TCGAPP;
using JustTCG;
using System.Net;

/// <summary>
/// 
/// </summary>
public class ApiMenu
{
    ///<param name="api">
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static IApi Run()
    {
        if (ApiManager.NumofApis == 0)
        {
            Console.WriteLine("Error: No avaible APIs [ApiMenu]");
            return null;
        }

        int chosenApi;

        while (true)
        {
            if (!Console.IsOutputRedirected && !Console.IsErrorRedirected)
            {
                Console.Clear();
            }
            Console.WriteLine("Select API to Search");
            Console.WriteLine($"Enter from range of 1-{ApiManager.NumofApis}\n");

            for (int i = 0; i < ApiManager.NumofApis; i++)
            {
                Console.WriteLine($"[ {i + 1} | {ApiManager.ApiRepositories[i].Name} ]");
            }

            Console.Write("\nEnter choice: ");
            string? input = Console.ReadLine();

            if (!int.TryParse(input, out chosenApi))
            {
                Console.WriteLine("That's not a valid choice.\n");
                Console.Write("Press Enter to continue...");
                Console.ReadLine();
                continue;
            }

            if (chosenApi < 1 || chosenApi > ApiManager.NumofApis)
            {
                Console.WriteLine($"Invalid selection. Please select from range 1-{ApiManager.NumofApis}\n");
                Console.Write("Press Enter to continue...");
                Console.ReadLine();
                continue;
            }

            break;
        }

        return ApiManager.ApiRepositories[chosenApi - 1];
    }
}
