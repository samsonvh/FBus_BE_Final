using FBus_BE.DTOs;
using FBus_BE.DTOs.InputDTOs;
using FBus_BE.DTOs.ListingDTOs;
using FBus_BE.DTOs.PageDTOs;
using FBus_BE.Models;

namespace FBus_BE.Services
{
    public interface IDriverService : IDefaultService<DriverDto, DriverListingDto, DriverInputDto, DriverPageRequest>
    {
    }
}
