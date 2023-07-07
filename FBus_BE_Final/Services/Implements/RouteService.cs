using AutoMapper;
using FBus_BE.DTOs;
using FBus_BE.DTOs.InputDTOs;
using FBus_BE.DTOs.ListingDTOs;
using FBus_BE.DTOs.PageDTOs;
using FBus_BE.Enums;
using FBus_BE.Exceptions;
using FBus_BE.Models;
using FBus_BE.Utils;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static System.Collections.Specialized.BitVector32;
using Route = FBus_BE.Models.Route;

namespace FBus_BE.Services.Implements
{
    public class RouteService : IRouteService
    {
        private Dictionary<string, string> errors;
        private readonly FbusMainContext _context;
        private readonly IMapper _mapper;
        private readonly Dictionary<string, Expression<Func<Route, object>>> _orderDict;

        public RouteService(FbusMainContext context, IMapper mapper)
        {
            errors = new Dictionary<string, string>();
            _context = context;
            _mapper = mapper;
            _orderDict = new Dictionary<string, Expression<Func<Route, object>>>
            {
                {"id", route => route.Id } 
            };
        }

        public async Task<bool> ChangeStatus(int id, string status)
        {
            Route? route = await _context.Routes.FirstOrDefaultAsync(route => route.Id == id);
            if (route == null)
            {
                if(route.Status != (byte)RouteStatusEnum.Deleted)
                {
                    status = TextUtil.Capitalize(status);
                    RouteStatusEnum statusEnum;
                    switch (status)
                    {
                        case nameof(RouteStatusEnum.Active):
                            statusEnum = RouteStatusEnum.Active;
                            break;
                        case nameof(RouteStatusEnum.Inactive):
                            statusEnum = RouteStatusEnum.Inactive;
                            break;
                        default:
                            return false;
                    }
                    route.Status = (byte)statusEnum;
                    _context.Routes.Update(route);
                    await _context.SaveChangesAsync();
                    return true;
                } else
                {
                    return false;
                }
            }
            else
            {
                throw new EntityNotFoundException("Route", id);
            }
        }

        public async Task<RouteDto> Create(int createdById, RouteInputDto inputDto)
        {
            Route route = _mapper.Map<Route>(inputDto);
            route.CreatedById = (short?)createdById;
            route.Status = (byte)RouteStatusEnum.Active;
            _context.Routes.Add(route);
            await _context.SaveChangesAsync();

            for(int i = 1; i <= inputDto.StationIds.Count; i++)
            {
                RouteStation routeStation = new RouteStation
                {
                    RouteId = route.Id,
                    StationId = (short?)inputDto.StationIds[i-1],
                    StationOrder = (byte)i
                };
                _context.RouteStations.Add(routeStation);
            }
            if(inputDto.StationIds != null)
            {
                await _context.SaveChangesAsync();
            }
            List<RouteStationDto> stations = await _context.RouteStations
                .Include(routeStation => routeStation.Station)
                .Where(routeStation => routeStation.RouteId == route.Id)
                .Select(routeStation => _mapper.Map<RouteStationDto>(new RouteStation
                {
                    Id = routeStation.Id,
                    RouteId = routeStation.RouteId,
                    StationId = routeStation.StationId,
                    Station = routeStation.Station,
                    StationOrder= routeStation.StationOrder
                }))
                .ToListAsync();
            RouteDto routeDto = _mapper.Map<RouteDto>(route);
            routeDto.Stations = stations;
            return routeDto;
        }

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<RouteDto> GetDetails(int id)
        {
            throw new NotImplementedException();
        }

        public Task<DefaultPageResponse<RouteListingDto>> GetList(RoutePageRequest pageRequest)
        {
            throw new NotImplementedException();
        }

        public Task<RouteDto> Update(int createdById, RouteInputDto inputDto, int id)
        {
            throw new NotImplementedException();
        }
    }
}
