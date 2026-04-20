using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TakedownTCG.Core.Abstractions;
using TakedownTCGApplication.ViewModels.Favorites;

namespace TakedownTCGApplication.Controllers;

[Authorize]
public sealed class FavoritesController : Controller
{
    private readonly IFavoriteService _favoriteService;

    public FavoritesController(IFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        string userName = User.Identity?.Name ?? string.Empty;
        FavoritesIndexViewModel model = new()
        {
            UserName = userName,
            Favorites = await _favoriteService.GetFavoritesAsync(userName)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(AddFavoriteViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Unable to add favorite with the provided data.";
            return RedirectToReturnUrl(model.ReturnUrl);
        }

        string userName = User.Identity?.Name ?? string.Empty;
        bool success = await _favoriteService.AddFavoriteAsync(userName, model.ItemType, model.ItemId, model.ItemName);
        TempData[success ? "StatusMessage" : "ErrorMessage"] = success
            ? "Added to favorites."
            : "Already favorited or failed to add.";

        return RedirectToReturnUrl(model.ReturnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(string itemType, string itemId)
    {
        string userName = User.Identity?.Name ?? string.Empty;
        bool success = await _favoriteService.RemoveFavoriteAsync(userName, itemType, itemId);
        TempData[success ? "StatusMessage" : "ErrorMessage"] = success
            ? "Favorite removed."
            : "Failed to remove favorite.";

        return RedirectToAction(nameof(Index));
    }

    private IActionResult RedirectToReturnUrl(string? returnUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Index));
    }
}
