using FBus_BE.DTOs;
using FBus_BE.DTOs.InputDTOs;
using FBus_BE.DTOs.PageDTOs;

namespace FBus_BE.Services
{
    public interface ITripForDriverService : IDefaultService<TripDto, TripDto, TripInputDto, TripPageRequest>
    {
        Task<DefaultPageResponse<TripDto>> GetList(int driverId, TripPageRequest pageRequest);
    }
}
