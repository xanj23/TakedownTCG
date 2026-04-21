namespace TakedownTCGApplication.ViewModels.Home;

public sealed class HomeIndexViewModel
{
    public IReadOnlyList<CompletedTcgSaleViewModel> CompletedSales { get; set; } = Array.Empty<CompletedTcgSaleViewModel>();
    public string CompletedSalesErrorMessage { get; set; } = string.Empty;
}
