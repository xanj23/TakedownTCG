using System;

namespace TakedownTCG.cli.Api
{
    public static class FavoriteController
    {
        public static void ShowFavoritesMenu()
        {
            var current = UserAccountController.CurrentUser;
            if (current is null)
            {
                Console.WriteLine("Login to view favorites.");
                return;
            }

            while (true)
            {
                var favs = UserAccountController.FavoriteService.GetFavorites(current.UserName);

                Console.WriteLine();
                Console.WriteLine($"Favorites for {current.UserName}:");

                if (favs.Count == 0)
                {
                    Console.WriteLine("No favorites yet.");
                }
                else
                {
                    for (int i = 0; i < favs.Count; i++)
                    {
                        var f = favs[i];
                        Console.WriteLine($"{i + 1}. [{f.ItemType}] {f.ItemName} (Id: {f.ItemId}) - Added: {f.CreatedAt:u}");
                    }
                }

                Console.WriteLine();
                Console.WriteLine("Options: (R)emove favorite, (B)ack");
                string choice = TakedownTCG.cli.Util.UserInput.InputString("Select option");
                if (string.IsNullOrWhiteSpace(choice))
                {
                    continue;
                }

                if (choice.Equals("B", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                if (choice.Equals("R", StringComparison.OrdinalIgnoreCase))
                {
                    if (favs.Count == 0)
                    {
                        Console.WriteLine("No favorites to remove.");
                        continue;
                    }

                    string idxStr = TakedownTCG.cli.Util.UserInput.InputRequiredString("Enter favorite number to remove");
                    if (!int.TryParse(idxStr, out int idx) || idx < 1 || idx > favs.Count)
                    {
                        Console.WriteLine("Invalid selection.");
                        continue;
                    }

                    var toRemove = favs[idx - 1];
                    bool removed = UserAccountController.FavoriteService.RemoveFavorite(current.UserName, toRemove.ItemType, toRemove.ItemId);
                    Console.WriteLine(removed ? "Favorite removed." : "Failed to remove favorite.");
                }
                else
                {
                    Console.WriteLine("Unknown option.");
                }
            }
        }
    }
}
