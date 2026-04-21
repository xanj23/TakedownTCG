using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TakedownTCGApplication.Abstractions;
using TakedownTCGApplication.Models;
using TakedownTCGApplication.Models.Home;
using TakedownTCGApplication.ViewModels.Home;

namespace TakedownTCGApplication.Controllers;

public class HomeController : Controller
{
    private readonly ICompletedTcgSalesService _completedSalesService;

    public HomeController(ICompletedTcgSalesService completedSalesService)
    {
        _completedSalesService = completedSalesService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        HomeIndexViewModel model = new();
        try
        {
            IReadOnlyList<CompletedTcgSale> completedSales = await _completedSalesService.GetRecentCompletedSalesAsync(cancellationToken);
            model.CompletedSales = completedSales.Select(ToViewModel).ToList();
        }
        catch (Exception ex)
        {
            model.CompletedSalesErrorMessage = $"Completed sales are unavailable right now: {ex.Message}";
        }

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private static CompletedTcgSaleViewModel ToViewModel(CompletedTcgSale sale)
    {
        return new CompletedTcgSaleViewModel
        {
            Title = sale.Title,
            ProductId = sale.ProductId,
            Url = sale.Url,
            ImageUrl = sale.ImageUrl,
            FallbackImageUrl = sale.FallbackImageUrl,
            Price = sale.Price,
            PriceText = sale.PriceText,
            Condition = sale.Condition,
            Seller = sale.Seller,
            Shipping = sale.Shipping
        };
    }
}
