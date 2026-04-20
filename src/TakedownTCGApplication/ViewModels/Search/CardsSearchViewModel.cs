using System.ComponentModel.DataAnnotations;
using TakedownTCG.Core.Models.JustTcg.Response;

namespace TakedownTCGApplication.ViewModels.Search;

public sealed class CardsSearchViewModel
{
    [Required]
    [Display(Name = "Name")]
    public string Query { get; set; } = string.Empty;

    [Display(Name = "Card Number")]
    public string Number { get; set; } = string.Empty;

    [Display(Name = "Printing")]
    public string Printing { get; set; } = string.Empty;

    [Display(Name = "Condition")]
    public string Condition { get; set; } = string.Empty;

    public IReadOnlyList<Card> Results { get; set; } = Array.Empty<Card>();
    public string ErrorMessage { get; set; } = string.Empty;
}
