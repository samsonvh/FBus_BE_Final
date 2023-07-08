using FBus_BE.DTOs;
using FBus_BE.DTOs.InputDTOs;
using FBus_BE.DTOs.ListingDTOs;
using FBus_BE.DTOs.PageDTOs;

namespace FBus_BE.Services
{
    public interface ICoordinationForDriverService
    {
        Task<CoordinationDto> GetDetails(int id, int driverId);
        Task<DefaultPageResponse<CoordinationListingDto>> GetList(int driverId, CoordinationPageRequest pageRequest);
    }
}
