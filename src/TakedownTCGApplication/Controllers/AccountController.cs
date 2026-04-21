using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TakedownTCG.Core.Abstractions;
using TakedownTCG.Core.Models.UserAccounts;
using TakedownTCGApplication.ViewModels.Account;

namespace TakedownTCGApplication.Controllers;

public sealed class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        bool success = await _accountService.CreateAccountAsync(model.UserName, model.Email, model.Password, true);
        if (!success)
        {
            ModelState.AddModelError(string.Empty, "Failed to create account. Username or email may already exist.");
            return View(model);
        }

        TempData["StatusMessage"] = "Account created successfully. Please sign in.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult Login(string returnUrl = "")
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        User? user = await _accountService.LoginAsync(model.UserNameOrEmail, model.Password);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Login failed. Please check credentials.");
            return View(model);
        }

        await SignInAsync(user.UserName, user.UserEmail);
        TempData["StatusMessage"] = $"Welcome back, {user.UserName}.";

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction(nameof(Profile));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        string? userName = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(userName))
        {
            return RedirectToAction(nameof(Login));
        }

        User? user = await _accountService.GetByUserNameAsync(userName);
        if (user is null)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        ProfileViewModel model = new()
        {
            UserName = user.UserName,
            Email = user.UserEmail,
            NotificationsEnabled = user.UserNotifications
        };

        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeUserName(ChangeUserNameViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Please provide a valid username.";
            return RedirectToAction(nameof(Profile));
        }

        string currentUserName = User.Identity?.Name ?? string.Empty;
        bool success = await _accountService.ChangeUserNameAsync(currentUserName, model.NewUserName);
        if (!success)
        {
            TempData["ErrorMessage"] = "Failed to update username. New name may already be taken.";
            return RedirectToAction(nameof(Profile));
        }

        User? user = await _accountService.GetByUserNameAsync(model.NewUserName);
        if (user is not null)
        {
            await SignInAsync(user.UserName, user.UserEmail);
        }

        TempData["StatusMessage"] = "Username updated successfully.";
        return RedirectToAction(nameof(Profile));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeEmail(ChangeEmailViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Please provide a valid email.";
            return RedirectToAction(nameof(Profile));
        }

        string userName = User.Identity?.Name ?? string.Empty;
        bool success = await _accountService.ChangeEmailAsync(userName, model.NewEmail);
        if (!success)
        {
            TempData["ErrorMessage"] = "Failed to update email. New email may already be taken.";
            return RedirectToAction(nameof(Profile));
        }

        User? user = await _accountService.GetByUserNameAsync(userName);
        if (user is not null)
        {
            await SignInAsync(user.UserName, user.UserEmail);
        }

        TempData["StatusMessage"] = "Email updated successfully.";
        return RedirectToAction(nameof(Profile));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Please provide both current and new password.";
            return RedirectToAction(nameof(Profile));
        }

        string userName = User.Identity?.Name ?? string.Empty;
        bool success = await _accountService.ChangePasswordAsync(userName, model.CurrentPassword, model.NewPassword);

        TempData[success ? "StatusMessage" : "ErrorMessage"] = success
            ? "Password changed successfully."
            : "Failed to change password. Current password may be incorrect.";

        return RedirectToAction(nameof(Profile));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleNotifications()
    {
        string userName = User.Identity?.Name ?? string.Empty;
        User? user = await _accountService.GetByUserNameAsync(userName);
        if (user is null)
        {
            TempData["ErrorMessage"] = "Unable to update notifications.";
            return RedirectToAction(nameof(Profile));
        }

        bool newSetting = !user.UserNotifications;
        bool success = await _accountService.UpdateUserNotificationsAsync(userName, newSetting);
        TempData[success ? "StatusMessage" : "ErrorMessage"] = success
            ? (newSetting ? "Notifications enabled." : "Notifications disabled.")
            : "Failed to update notification settings.";

        return RedirectToAction(nameof(Profile));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAccount(DeleteAccountViewModel model)
    {
        if (!string.Equals(model.Confirmation, "DELETE", StringComparison.OrdinalIgnoreCase))
        {
            TempData["ErrorMessage"] = "Account deletion cancelled. Confirmation text did not match.";
            return RedirectToAction(nameof(Profile));
        }

        string userName = User.Identity?.Name ?? string.Empty;
        bool success = await _accountService.DeleteAccountAsync(userName);
        if (!success)
        {
            TempData["ErrorMessage"] = "Failed to delete account.";
            return RedirectToAction(nameof(Profile));
        }

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["StatusMessage"] = "Account deleted successfully.";
        return RedirectToAction(nameof(Register));
    }

    private async Task SignInAsync(string userName, string userEmail)
    {
        List<Claim> claims =
        [
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.Email, userEmail)
        ];

        ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        ClaimsPrincipal principal = new(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = false,
                AllowRefresh = true
            });
    }
}
