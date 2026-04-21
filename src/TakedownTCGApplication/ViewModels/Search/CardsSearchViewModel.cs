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

    public int Offset { get; set; }
    public int Limit { get; set; } = 20;
    public int NextOffset { get; set; }
    public int PreviousOffset { get; set; }
    public int Total { get; set; }
    public bool HasMore { get; set; }
    public bool HasPrevious { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }

    public IReadOnlyList<Card> Results { get; set; } = Array.Empty<Card>();
    public string ErrorMessage { get; set; } = string.Empty;
}
