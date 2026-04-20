using System.ComponentModel.DataAnnotations;

namespace TakedownTCGApplication.ViewModels.Account;

public sealed class LoginViewModel
{
    [Required]
    [Display(Name = "Username or Email")]
    public string UserNameOrEmail { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string ReturnUrl { get; set; } = string.Empty;
}
