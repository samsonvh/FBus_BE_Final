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
                if (route.Status != (byte)RouteStatusEnum.Deleted)
                {
                    status = TextUtil.Capitalize(status);
                    RouteStatusEnum statusEnum;
                    switch (status)
                    {
                        case nameof(RouteStatusEnum.Active):
                            int stationCount = await _context.RouteStations.Where(routeStation => routeStation.RouteId == id).CountAsync();
                            if (stationCount >= 2)
                            {
                                statusEnum = RouteStatusEnum.Active;
                            }
                            else
                            {
                                throw new NotEnoughStationForRouteException(id, stationCount);
                            }
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
                }
                else
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
            route.Status = (byte)RouteStatusEnum.Inactive;
            _context.Routes.Add(route);
            await _context.SaveChangesAsync();
            RouteDto routeDto = _mapper.Map<RouteDto>(route);
            
            if (inputDto.StationIds != null)
            {
                for (int i = 1; i <= inputDto.StationIds.Count; i++)
                {
                    RouteStation routeStation = new RouteStation
                    {
                        RouteId = route.Id,
                        StationId = (short?)inputDto.StationIds[i - 1],
                        StationOrder = (byte)i
                    };
                    _context.RouteStations.Add(routeStation);
                }
                await _context.SaveChangesAsync();
                List<RouteStationDto> stations = await _context.RouteStations
                    .Include(routeStation => routeStation.Station)
                    .Where(routeStation => routeStation.RouteId == route.Id)
                    .Select(routeStation => _mapper.Map<RouteStationDto>(new RouteStation
                    {
                        Id = routeStation.Id,
                        RouteId = routeStation.RouteId,
                        StationId = routeStation.StationId,
                        Station = routeStation.Station,
                        StationOrder = routeStation.StationOrder
                    }))
                    .ToListAsync();
                if (stations.Count >= 2)
                {
                    route.Status = (byte)RouteStatusEnum.Active;
                    _context.Routes.Update(route);
                    await _context.SaveChangesAsync();
                }
                routeDto.Stations = stations;
            }
            return routeDto;
        }

        public async Task<bool> Delete(int id)
        {
            Route? route = await _context.Routes.FirstOrDefaultAsync(route => route.Id == id);
            if (route != null)
            {
                if (route.Status != (byte)RouteStatusEnum.Deleted)
                {
                    route.Status = (byte)RouteStatusEnum.Deleted;
                    _context.Routes.Update(route);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                throw new EntityNotFoundException("Route", id);
            }
        }

        public async Task<RouteDto> GetDetails(int id)
        {
            Route? route = await _context.Routes
                .Include(route => route.CreatedBy)
                .FirstOrDefaultAsync(route => route.Id == id);
            if (route != null)
            {
                List<RouteStationDto> stations = await _context.RouteStations
                .Include(routeStation => routeStation.Station)
                .Where(routeStation => routeStation.RouteId == route.Id)
                .Select(routeStation => _mapper.Map<RouteStationDto>(new RouteStation
                {
                    Id = routeStation.Id,
                    RouteId = routeStation.RouteId,
                    StationId = routeStation.StationId,
                    Station = routeStation.Station,
                    StationOrder = routeStation.StationOrder
                }))
                .ToListAsync();
                RouteDto routeDto = _mapper.Map<RouteDto>(route);
                routeDto.Stations = stations;
                return routeDto;
            }
            else
            {
                throw new EntityNotFoundException("Route", id);
            }
        }

        public async Task<DefaultPageResponse<RouteListingDto>> GetList(RoutePageRequest pageRequest)
        {
            DefaultPageResponse<RouteListingDto> pageResponse = new DefaultPageResponse<RouteListingDto>();
            if (pageRequest.PageIndex == null)
            {
                pageRequest.PageIndex = 1;
            }
            if (pageRequest.PageSize == null)
            {
                pageRequest.PageSize = 10;
            }
            if (pageRequest.OrderBy == null)
            {
                pageRequest.OrderBy = "id";
            }
            int skippedCount = (int)((pageRequest.PageIndex - 1) * pageRequest.PageSize);
            List<RouteListingDto> routes = new List<RouteListingDto>();
            int totalCount = await _context.Routes
                .Where(route => route.Status != (byte)RouteStatusEnum.Deleted)
                .Where(route => (pageRequest.Beginning != null && pageRequest.Destination != null)
                               ? route.Beginning.Contains(pageRequest.Beginning) || route.Destination.Contains(pageRequest.Destination)
                               : (pageRequest.Beginning != null && pageRequest.Destination == null)
                                  ? route.Beginning.Contains(pageRequest.Beginning)
                                  : (pageRequest.Beginning == null && pageRequest.Destination != null)
                                     ? route.Destination.Contains(pageRequest.Destination)
                                     : true)
                .CountAsync();
            if (totalCount > 0)
            {
                routes = pageRequest.Direction == "desc"
                    ? await _context.Routes.OrderByDescending(_orderDict[pageRequest.OrderBy.ToLower()])
                                           .Skip(skippedCount)
                                           .Where(route => route.Status != (byte)RouteStatusEnum.Deleted)
                                           .Where(route => (pageRequest.Beginning != null && pageRequest.Destination != null)
                                                            ? route.Beginning.Contains(pageRequest.Beginning) || route.Destination.Contains(pageRequest.Destination)
                                                            : (pageRequest.Beginning != null && pageRequest.Destination == null)
                                                               ? route.Beginning.Contains(pageRequest.Beginning)
                                                               : (pageRequest.Beginning == null && pageRequest.Destination != null)
                                                                  ? route.Destination.Contains(pageRequest.Destination)
                                                                  : true)
                                           .Select(route => _mapper.Map<RouteListingDto>(route))
                                           .ToListAsync()
                    : await _context.Routes.OrderBy(_orderDict[pageRequest.OrderBy.ToLower()])
                                           .Skip(skippedCount)
                                           .Where(route => route.Status != (byte)RouteStatusEnum.Deleted)
                                           .Where(route => (pageRequest.Beginning != null && pageRequest.Destination != null)
                                                            ? route.Beginning.Contains(pageRequest.Beginning) || route.Destination.Contains(pageRequest.Destination)
                                                            : (pageRequest.Beginning != null && pageRequest.Destination == null)
                                                               ? route.Beginning.Contains(pageRequest.Beginning)
                                                               : (pageRequest.Beginning == null && pageRequest.Destination != null)
                                                                  ? route.Destination.Contains(pageRequest.Destination)
                                                                  : true)
                                           .Select(route => _mapper.Map<RouteListingDto>(route))
                                           .ToListAsync();
            }
            pageResponse.Data = routes;
            pageResponse.PageSize = (int)pageRequest.PageSize;
            pageResponse.PageCount = (int)(totalCount / pageRequest.PageSize) + 1;
            pageResponse.PageSize = (int)pageRequest.PageSize;
            return pageResponse;
        }

        public async Task<RouteDto> Update(int createdById, RouteInputDto inputDto, int id)
        {
            Route? route = await _context.Routes
                .Include(route => route.CreatedBy)
                .Include(route => route.RouteStations)
                .FirstOrDefaultAsync(route => route.Id == id);
            if(route != null)
            {
                route = _mapper.Map(inputDto, route);
                route.Status = (byte)RouteStatusEnum.Inactive;
                _context.Routes.Update(route);
                await _context.SaveChangesAsync();
                RouteDto routeDto = _mapper.Map<RouteDto>(route);

                _context.RouteStations.RemoveRange(route.RouteStations);
                for (int i = 1; i <= inputDto.StationIds.Count; i++)
                {
                    RouteStation routeStation = new RouteStation
                    {
                        RouteId = route.Id,
                        StationId = (short?)inputDto.StationIds[i - 1],
                        StationOrder = (byte)i
                    };
                    _context.RouteStations.Add(routeStation);
                }
                if (inputDto.StationIds != null)
                {
                    await _context.SaveChangesAsync();
                    List<RouteStationDto> stations = await _context.RouteStations
                        .Include(routeStation => routeStation.Station)
                        .Where(routeStation => routeStation.RouteId == route.Id)
                        .Select(routeStation => _mapper.Map<RouteStationDto>(new RouteStation
                        {
                            Id = routeStation.Id,
                            RouteId = routeStation.RouteId,
                            StationId = routeStation.StationId,
                            Station = routeStation.Station,
                            StationOrder = routeStation.StationOrder
                        }))
                        .ToListAsync();
                    if (stations.Count >= 2)
                    {
                        route.Status = (byte)RouteStatusEnum.Active;
                        _context.Routes.Update(route);
                        await _context.SaveChangesAsync();
                    }
                    routeDto.Stations = stations;
                }
                return routeDto;
            } else
            {
                throw new EntityNotFoundException("Route", id);
            }
        }
    }
}
