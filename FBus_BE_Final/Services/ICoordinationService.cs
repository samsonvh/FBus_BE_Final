using FBus_BE.DTOs;
using FBus_BE.DTOs.InputDTOs;
using FBus_BE.DTOs.ListingDTOs;
using FBus_BE.DTOs.PageDTOs;

namespace FBus_BE.Services
{
    public interface ICoordinationService : IDefaultService<CoordinationDto, CoordinationListingDto, CoordinationInputDto, CoordinationPageRequest>
    {
    }
}
