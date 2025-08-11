using Garage.Shared.Models;

namespace Garage.Web.Services;

public class WinnersApiClient(HttpClient httpClient)
{
    public async Task<Winner[]> GetAllWinnersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var winners = await httpClient.GetFromJsonAsync<Winner[]>("/lemans/winners", cancellationToken);
            return winners ?? [];
        }
        catch (Exception)
        {
            // Return empty array if API call fails
            return [];
        }
    }

    public async Task<Winner?> GetWinnerByYearAsync(int year, CancellationToken cancellationToken = default)
    {
        try
        {
            return await httpClient.GetFromJsonAsync<Winner>($"/lemans/winners/{year}", cancellationToken);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<Winner[]> GetWinnersByManufacturerAsync(string manufacturer, CancellationToken cancellationToken = default)
    {
        try
        {
            var winners = await httpClient.GetFromJsonAsync<Winner[]>($"/lemans/winners/manufacturer/{manufacturer}", cancellationToken);
            return winners ?? [];
        }
        catch (Exception)
        {
            return [];
        }
    }

    public async Task<Winner[]> GetWinnersByClassAsync(string carClass, CancellationToken cancellationToken = default)
    {
        try
        {
            var winners = await httpClient.GetFromJsonAsync<Winner[]>($"/lemans/winners/class/{carClass}", cancellationToken);
            return winners ?? [];
        }
        catch (Exception)
        {
            return [];
        }
    }
}
