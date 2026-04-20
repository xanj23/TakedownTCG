using TakedownTCG.Core.Models.UserAccounts;

namespace TakedownTCGApplication.ViewModels.Favorites;

public sealed class FavoritesIndexViewModel
{
    public string UserName { get; set; } = string.Empty;
    public IReadOnlyList<Favorite> Favorites { get; set; } = Array.Empty<Favorite>();
}
