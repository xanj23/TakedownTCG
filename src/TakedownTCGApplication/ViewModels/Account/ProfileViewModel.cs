namespace TakedownTCGApplication.ViewModels.Account;

public sealed class ProfileViewModel
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool NotificationsEnabled { get; set; }
}
