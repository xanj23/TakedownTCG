using System.ComponentModel.DataAnnotations;

namespace TakedownTCGApplication.ViewModels.Account;

public sealed class ChangeUserNameViewModel
{
    [Required]
    [StringLength(64, MinimumLength = 3)]
    [Display(Name = "New Username")]
    public string NewUserName { get; set; } = string.Empty;
}
