using AutoMapper;
using FBus_BE.DTOs;
using FBus_BE.DTOs.InputDTOs;
using FBus_BE.DTOs.ListingDTOs;
using FBus_BE.DTOs.PageDTOs;
using FBus_BE.Enums;
using FBus_BE.Exceptions;
using FBus_BE.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Route = FBus_BE.Models.Route;

namespace FBus_BE.Services.Implements
{
    public class RouteForMapScreenService : IRouteForMapScreenService
    {
        private Dictionary<string, string> errors;
        private readonly FbusMainContext _context;
        private readonly IMapper _mapper;
        private readonly Dictionary<string, Expression<Func<Route, object>>> _orderDict;

        public RouteForMapScreenService(FbusMainContext context, IMapper mapper)
        {
            errors = new Dictionary<string, string>();
            _context = context;
            _mapper = mapper;
            _orderDict = new Dictionary<string, Expression<Func<Route, object>>>
            {
                {"id", route => route.Id }
            };
        }

        public Task<bool> ChangeStatus(int id, string status)
        {
            throw new NotImplementedException();
        }

        public Task<RouteDto> Create(int createdById, RouteInputDto inputDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<RouteDto> GetDetails(int id)
        {
            Route? route = await _context.Routes
                .Include(route => route.CreatedBy)
                .FirstOrDefaultAsync(route => route.Id == id && route.Status == (byte)RouteStatusEnum.Active);
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
                routeDto.RouteStations = stations;
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
                .Where(route => route.Status == (byte)RouteStatusEnum.Active)
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
                                           .Where(route => route.Status == (byte)RouteStatusEnum.Active)
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
                                           .Where(route => route.Status == (byte)RouteStatusEnum.Active)
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

        public Task<RouteDto> Update(int createdById, RouteInputDto inputDto, int id)
        {
            throw new NotImplementedException();
        }
    }
}
