using System.ComponentModel.DataAnnotations;
using TakedownTCG.Core.Models.JustTcg.Response;

namespace TakedownTCGApplication.ViewModels.Search;

public sealed class CardsSearchViewModel
{
    [Required]
    [Display(Name = "Name")]
    public string? Query { get; set; }

    [Display(Name = "Card Number")]
    public string? Number { get; set; }

    [Display(Name = "Printing")]
    public string? Printing { get; set; }

    [Display(Name = "Condition")]
    public string? Condition { get; set; }

    public IReadOnlyList<Card> Results { get; set; } = Array.Empty<Card>();
    public string ErrorMessage { get; set; } = string.Empty;
}
