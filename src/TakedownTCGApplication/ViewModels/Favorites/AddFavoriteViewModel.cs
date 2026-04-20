using System.ComponentModel.DataAnnotations;

namespace TakedownTCGApplication.ViewModels.Favorites;

public sealed class AddFavoriteViewModel
{
    [Required]
    public string ItemType { get; set; } = string.Empty;

    [Required]
    public string ItemId { get; set; } = string.Empty;

    [Required]
    public string ItemName { get; set; } = string.Empty;

    public string ReturnUrl { get; set; } = string.Empty;
}
