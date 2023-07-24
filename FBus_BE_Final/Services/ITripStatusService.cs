using FBus_BE.DTOs;
using FBus_BE.DTOs.InputDTOs;
using FBus_BE.DTOs.PageDTOs;

namespace FBus_BE.Services
{
    public interface ITripStatusService : IDefaultService<TripStatusDto, TripStatusDto, TripStatusInputDto, TripStatusPageRequest>
    {
    }
}
