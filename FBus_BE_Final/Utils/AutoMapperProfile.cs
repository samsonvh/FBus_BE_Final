using AutoMapper;
using FBus_BE.DTOs;
using FBus_BE.DTOs.InputDTOs;
using FBus_BE.DTOs.ListingDTOs;
using FBus_BE.Enums;
using FBus_BE.Models;
using Route = FBus_BE.Models.Route;

namespace FBus_BE.Utils
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //  Account
            CreateMap<Account, AccountDto>()
                .ForMember(accountDto => accountDto.Status, options => options.MapFrom(account => MapAccountStatus(account.Status)));

            //  Driver
            CreateMap<Driver, DriverDto>()
                .ForMember(driverDto => driverDto.Code, options => options.MapFrom(driver => driver.Account.Code))
                .ForMember(driverDto => driverDto.Email, options => options.MapFrom(driver => driver.Account.Email))
                .ForMember(driverDto => driverDto.Gender, options => options.MapFrom(driver => MapGender(driver.Gender)))
                .ForMember(driverDto => driverDto.CreatedByCode, options => options.MapFrom(driver => driver.CreatedBy.Code))
                .ForMember(driverDto => driverDto.Status, options => options.MapFrom(driver => MapDriverStatus(driver.Status)));
            CreateMap<Driver, DriverListingDto>()
                .ForMember(driverDto => driverDto.Code, options => options.MapFrom(driver => driver.Account.Code))
                .ForMember(driverDto => driverDto.Email, options => options.MapFrom(driver => driver.Account.Email))
                .ForMember(driverDto => driverDto.Gender, options => options.MapFrom(driver => MapGender(driver.Gender)))
                .ForMember(driverDto => driverDto.Status, options => options.MapFrom(driver => MapDriverStatus(driver.Status)));
            CreateMap<DriverInputDto, Driver>();

            //  Bus
            CreateMap<Bus, BusDto>()
                .ForMember(busDto => busDto.CreatedByCode, options => options.MapFrom(bus => bus.CreatedBy.Code))
                .ForMember(busDto => busDto.Status, options => options.MapFrom(bus => MapBusStatus(bus.Status)));
            CreateMap<Bus, BusListingDto>()
                .ForMember(busDto => busDto.Status, options => options.MapFrom(bus => MapBusStatus(bus.Status)));
            CreateMap<BusInputDto, Bus>();

            //  Station
            CreateMap<Station, StationDto>()
                .ForMember(stationDto => stationDto.CreatedByCode, options => options.MapFrom(station => station.CreatedBy.Code))
                .ForMember(stationDto => stationDto.Status, options => options.MapFrom(station => MapStationStatus(station.Status)));
            CreateMap<Station, StationListingDto>()
                .ForMember(stationDto => stationDto.Status, options => options.MapFrom(station => MapStationStatus(station.Status)));
            CreateMap<StationInputDto, Station>()
                .ForMember(station => station.Image, options => options.Ignore());

            //  Route
            CreateMap<Route, RouteDto>()
                .ForMember(routeDto => routeDto.CreatedByCode, options => options.MapFrom(route => route.CreatedBy.Code))
                .ForMember(routeDto => routeDto.Status, options => options.MapFrom(route => MapRouteStatus(route.Status)));
            CreateMap<RouteInputDto, Route>();

            //  RouteStation
            CreateMap<RouteStation, RouteStationDto>();

            //  Coordination
            CreateMap<Coordination, CoordinationDto>()
                .ForMember(coorDto => coorDto.CreatedByCode, options => options.MapFrom(coor => coor.CreatedBy.Code))
                .ForMember(coorDto => coorDto.Status, options => options.MapFrom(coor => MapCoordinationStatus(coor.Status)));
            CreateMap<Coordination, CoordinationListingDto>()
                .ForMember(coorDto => coorDto.BusCode, options => options.MapFrom(coor => coor.Bus.Code))
                .ForMember(coorDto => coorDto.LicensePlate, options => options.MapFrom(coor => coor.Bus.LicensePlate))
                .ForMember(coorDto => coorDto.DriverCode, options => options.MapFrom(coor => coor.Driver.Account.Code))
                .ForMember(coorDto => coorDto.Beginning, options => options.MapFrom(coor => coor.Route.Beginning))
                .ForMember(coorDto => coorDto.Destination, options => options.MapFrom(coor => coor.Route.Destination))
                .ForMember(coorDto => coorDto.Status, options => options.MapFrom(coor => MapCoordinationStatus(coor.Status)));
            CreateMap<CoordinationInputDto, Coordination>();
        }

        private static string MapAccountStatus(byte status)
        {
            switch (status)
            {
                case (int)AccountStatusEnum.Active:
                    return "ACTIVE";
                case (int)AccountStatusEnum.Inactive:
                    return "INACTIVE";
                case (int)AccountStatusEnum.Unsigned:
                    return "UNSIGNED";
                default:
                    return "DELETED";
            }
        }

        private static string MapDriverStatus(byte status)
        {
            switch (status)
            {
                case (int)DriverStatusEnum.Active:
                    return "ACTIVE";
                case (int)DriverStatusEnum.Inactive:
                    return "INACTIVE";
                default:
                    return "DELETED";
            }
        }

        private static string MapGender(bool? genderBool)
        {
            if (genderBool.Value)
            {
                return "Male";
            }
            else
            {
                return "Female";
            }
        }

        private static string MapBusStatus(byte status)
        {
            switch (status)
            {
                case (int)BusStatusEnum.Active:
                    return "ACTIVE";
                case (int)BusStatusEnum.Inactive:
                    return "INACTIVE";
                case (int)BusStatusEnum.OnGoing:
                    return "ONGOING";
                default:
                    return "DELETED";
            }
        }

        private static string MapStationStatus(byte status)
        {
            switch (status)
            {
                case (int)StationStatusEnum.Active:
                    return "ACTIVE";
                case (int)StationStatusEnum.Inactive:
                    return "INACTIVE";
                default:
                    return "DELETED";
            }
        }

        private static string MapRouteStatus(byte status)
        {
            switch (status)
            {
                case (int)RouteStatusEnum.Active:
                    return "ACTIVE";
                case (int)RouteStatusEnum.Inactive:
                    return "INACTIVE";
                default:
                    return "DELETED";
            }
        }

        private static string MapCoordinationStatus(byte status)
        {
            switch (status)
            {
                case (int)CoordinationStatusEnum.Active:
                    return "ACTIVE";
                case (int)CoordinationStatusEnum.Inactive:
                    return "INACTIVE";
                case (int)CoordinationStatusEnum.OnGoing:
                    return "ONGOING";
                case (int)CoordinationStatusEnum.Finished:
                    return "FINISHED";
                default:
                    return "DELETED";
            }
        }
    }
}
