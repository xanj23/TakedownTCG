using System.ComponentModel.DataAnnotations;

namespace TakedownTCGApplication.ViewModels.Account;

public sealed class DeleteAccountViewModel
{
    [Required]
    [Display(Name = "Type DELETE to confirm")]
    public string Confirmation { get; set; } = string.Empty;
}
