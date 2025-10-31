using DataModels = Garage.ApiService.Data.Models;
using Riok.Mapperly.Abstractions;
using SharedModels = Garage.Shared.Models;

[Mapper]
public partial class WinnerMapper
{
    [MapperIgnoreTarget(nameof(SharedModels.Winner.Image))]
    public partial SharedModels.Winner WinnerToWinnerDto(DataModels.Winner winner);
}
