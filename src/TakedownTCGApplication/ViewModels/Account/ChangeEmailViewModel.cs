using System.ComponentModel.DataAnnotations;

namespace TakedownTCGApplication.ViewModels.Account;

public sealed class ChangeEmailViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "New Email")]
    public string NewEmail { get; set; } = string.Empty;
}
