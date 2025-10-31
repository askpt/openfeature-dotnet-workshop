using Riok.Mapperly.Abstractions;
using DataModels = Garage.ApiService.Data.Models;
using SharedModels = Garage.Shared.Models;

[Mapper]
public partial class WinnerMapper
{
    [MapperIgnoreTarget(nameof(SharedModels.Winner.Image))]
    public partial SharedModels.Winner WinnerToWinnerDto(DataModels.Winner winner);
}
