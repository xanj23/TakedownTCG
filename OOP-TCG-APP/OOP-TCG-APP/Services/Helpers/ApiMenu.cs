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
        Console.Clear();

        if (ApiManager.NumofApis == null || ApiManager.NumofApis == 0)
        {
            Console.WriteLine("Error: No avaible APIs");
            return null;
        }

        Console.WriteLine($"Select API to Search");
        Console.WriteLine($"Enter from range of 1-{ApiManager.NumofApis}\n");

        int chosenApi;

        while (true)
        {
            for (int i = 0; i < ApiManager.NumofApis; i++)
            {
                Console.WriteLine($"[ {i + 1} | {ApiManager.ApiRepositories[i].Name} ]");
            }

            Console.Write("\nEnter choice: ");
            string? input = Console.ReadLine();

            if (!int.TryParse(input, out chosenApi))
            {
                Console.Clear();
                Console.WriteLine("That's not a valid choice.\n");
                continue;
            }

            if (chosenApi < 1 || chosenApi > ApiManager.NumofApis)
            {
                Console.Clear();
                Console.WriteLine($"Invalid selection. Please select from range 1-{ApiManager.NumofApis}\n");
                continue;
            }

            break;
        }

        return ApiManager.ApiRepositories[chosenApi - 1];
    }
}
