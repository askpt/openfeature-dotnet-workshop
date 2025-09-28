using Microsoft.AspNetCore.Mvc;
using Garage.React.Services;
using Garage.Shared.Models;

namespace Garage.React.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WinnersController : ControllerBase
{
    private readonly WinnersApiClient _winnersApiClient;

    public WinnersController(WinnersApiClient winnersApiClient)
    {
        _winnersApiClient = winnersApiClient;
    }

    [HttpGet]
    public async Task<Winner[]> GetAllWinners(CancellationToken cancellationToken = default)
    {
        return await _winnersApiClient.GetAllWinnersAsync(cancellationToken);
    }

    [HttpGet("{year}")]
    public async Task<Winner?> GetWinnerByYear(int year, CancellationToken cancellationToken = default)
    {
        return await _winnersApiClient.GetWinnerByYearAsync(year, cancellationToken);
    }

    [HttpGet("manufacturer/{manufacturer}")]
    public async Task<Winner[]> GetWinnersByManufacturer(string manufacturer, CancellationToken cancellationToken = default)
    {
        return await _winnersApiClient.GetWinnersByManufacturerAsync(manufacturer, cancellationToken);
    }

    [HttpGet("class/{carClass}")]
    public async Task<Winner[]> GetWinnersByClass(string carClass, CancellationToken cancellationToken = default)
    {
        return await _winnersApiClient.GetWinnersByClassAsync(carClass, cancellationToken);
    }
}