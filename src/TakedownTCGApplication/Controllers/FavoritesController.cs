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
            if (IsAjaxRequest())
            {
                return BadRequest(new { success = false, message = "Unable to add favorite with the provided data." });
            }

            TempData["ErrorMessage"] = "Unable to add favorite with the provided data.";
            return RedirectToReturnUrl(model.ReturnUrl);
        }

        string userName = User.Identity?.Name ?? string.Empty;
        bool success = await _favoriteService.AddFavoriteAsync(userName, model.ItemType, model.ItemId, model.ItemName);

        if (IsAjaxRequest())
        {
            return Ok(new
            {
                success,
                message = success ? "Added to favorites." : "Already favorited or failed to add."
            });
        }

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

        if (IsAjaxRequest())
        {
            return Ok(new
            {
                success,
                message = success ? "Favorite removed." : "Failed to remove favorite."
            });
        }

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

    private bool IsAjaxRequest()
    {
        return Request.Headers.TryGetValue("X-Requested-With", out var headerValue)
               && string.Equals(headerValue, "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
    }
}
