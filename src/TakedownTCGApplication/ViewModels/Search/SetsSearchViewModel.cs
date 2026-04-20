using System.ComponentModel.DataAnnotations;
using TakedownTCG.Core.Models.JustTcg.Response;

namespace TakedownTCGApplication.ViewModels.Search;

public sealed class SetsSearchViewModel
{
    [Required]
    [Display(Name = "Game")]
    public string Game { get; set; } = string.Empty;

    [Display(Name = "Search Query")]
    public string Query { get; set; } = string.Empty;

    [Display(Name = "Order By")]
    public string OrderBy { get; set; } = string.Empty;

    [Display(Name = "Sort Order")]
    public string Order { get; set; } = string.Empty;

    public IReadOnlyList<Set> Results { get; set; } = Array.Empty<Set>();
    public string ErrorMessage { get; set; } = string.Empty;
}
