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
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Linq.Expressions;
using Route = FBus_BE.Models.Route;

namespace FBus_BE.Services.Implements
{
    public class StationService : IStationService
    {
        private Dictionary<string, string> errors;
        private readonly FbusMainContext _context;
        private readonly IMapper _mapper;
        private readonly Dictionary<string, Expression<Func<Station, object>>> _orderDict;
        private readonly IFirebaseStorageService _storageService;
        private const string cloudStoragePrefix = @"https://firebasestorage.googleapis.com/v0/b/fbus-388009.appspot.com/o/";

        public StationService(FbusMainContext context, IMapper mapper, IFirebaseStorageService storageService)
        {
            errors = new Dictionary<string, string>();
            _context = context;
            _mapper = mapper;
            _orderDict = new Dictionary<string, Expression<Func<Station, object>>> {
                { "id", station => station.Id }
            };
            _storageService = storageService;
        }

        public async Task<bool> ChangeStatus(int id, string status)
        {
            Station? station = await _context.Stations.FirstOrDefaultAsync(station => station.Id == id);
            if (station != null)
            {
                if (station.Status != (byte)StationStatusEnum.Deleted)
                {
                    List<RouteStation> routeStations = await _context.RouteStations
                        .Where(routeStation => routeStation.StationId == id)
                        .ToListAsync();
                    List<Int32> routeIds = new List<int>();
                    foreach (RouteStation routeStation in routeStations)
                    {
                        routeIds.Add((int)routeStation.RouteId);
                    }
                    int onGoingTrips = 0;
                    foreach (int routeId in routeIds)
                    {
                        int onGoingRoutes = await _context.Trips.Where(trip => trip.RouteId == routeId).CountAsync();
                        if (onGoingRoutes > 0)
                        {
                            onGoingTrips++;
                        }
                    }
                    status = TextUtil.Capitalize(status);
                    StationStatusEnum statusEnum;
                    switch (status)
                    {
                        case nameof(StationStatusEnum.Active):
                            statusEnum = StationStatusEnum.Active;
                            break;
                        case nameof(StationStatusEnum.Inactive):
                            if (onGoingTrips > 0)
                            {
                                return false;
                            }
                            else
                            {
                                statusEnum = StationStatusEnum.Inactive;
                                break;
                            }
                        default:
                            return false;
                    }
                    station.Status = (byte)statusEnum;
                    _context.Stations.Update(station);
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
                throw new EntityNotFoundException("Station", id);
            }
        }

        public async Task<StationDto> Create(int createdById, StationInputDto inputDto)
        {
            await CheckCreateDuplicate(inputDto);
            if (errors.IsNullOrEmpty())
            {
                Station station = _mapper.Map<Station>(inputDto);
                station.CreatedById = (short?)createdById;
                station.Status = (byte)StationStatusEnum.Active;
                if (inputDto.Image != null)
                {
                    Uri uri = await _storageService.UploadFile(inputDto.Code, inputDto.Image, "stations");
                    station.Image = cloudStoragePrefix + uri.AbsolutePath.Substring(uri.AbsolutePath.LastIndexOf('/') + 1) + "?alt=media";
                }

                _context.Stations.Add(station);
                await _context.SaveChangesAsync();
                return _mapper.Map<StationDto>(station);
            }
            else
            {
                throw new DuplicateException(errors);
            }
        }

        public async Task<bool> Delete(int id)
        {
            Station? station = await _context.Stations.FirstOrDefaultAsync(station => station.Id == id);
            if (station != null)
            {
                if (station.Status != (byte)StationStatusEnum.Deleted)
                {
                    List<RouteStation> routeStations = await _context.RouteStations
                        .Where(routeStation => routeStation.StationId == id)
                        .ToListAsync();
                    List<Int32> routeIds = new List<int>();
                    foreach (RouteStation routeStation in routeStations)
                    {
                        routeIds.Add((int)routeStation.RouteId);
                    }
                    int onGoingTrips = 0;
                    foreach (int routeId in routeIds)
                    {
                        int onGoingRoutes = await _context.Trips.Where(trip => trip.RouteId == routeId).CountAsync();
                        if (onGoingRoutes > 0)
                        {
                            onGoingTrips++;
                        }
                    }
                    if (onGoingTrips > 0)
                    {
                        return false;
                    }
                    else
                    {
                        station.Status = (byte)StationStatusEnum.Deleted;
                        _context.Stations.Update(station);
                        await _context.SaveChangesAsync();
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                throw new EntityNotFoundException("Station", id);
            }
        }

        public async Task<StationDto> GetDetails(int id)
        {
            Station? station = await _context.Stations
                .Include(station => station.CreatedBy)
                .FirstOrDefaultAsync(station => station.Id == id);
            if (station != null)
            {
                return _mapper.Map<StationDto>(station);
            }
            else
            {
                throw new EntityNotFoundException("Station", id);
            }
        }

        public async Task<DefaultPageResponse<StationListingDto>> GetList(StationPageRequest pageRequest)
        {
            DefaultPageResponse<StationListingDto> pageResponse = new DefaultPageResponse<StationListingDto>();
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
            StationStatusEnum statusEnum = StationStatusEnum.Active;
            bool validStatus = false;
            if (pageRequest.Status != null)
            {
                switch (TextUtil.Capitalize(pageRequest.Status))
                {
                    case nameof(StationStatusEnum.Active):
                        statusEnum = StationStatusEnum.Active;
                        validStatus = true;
                        break;
                    case nameof(StationStatusEnum.Inactive):
                        statusEnum = StationStatusEnum.Inactive;
                        validStatus = true;
                        break;
                }
            }
            int skippedCount = (int)((pageRequest.PageIndex - 1) * pageRequest.PageSize);
            List<StationListingDto> stations = new List<StationListingDto>();
            int totalCount = await _context.Stations
                .Where(station => (validStatus) ? station.Status == (byte)statusEnum : true)
                .Where(station => pageRequest.Code != null ? station.Code.Contains(pageRequest.Code) : true)
                .CountAsync();
            if (totalCount > 0)
            {
                stations = pageRequest.Direction == "desc"
                    ? await _context.Stations.OrderByDescending(_orderDict[pageRequest.OrderBy.ToLower()])
                                             .Skip(skippedCount)
                                             .Where(station => (validStatus) ? station.Status == (byte)statusEnum : true)
                                             .Where(station => pageRequest.Code != null ? station.Code.Contains(pageRequest.Code) : true)
                                             .Select(station => _mapper.Map<StationListingDto>(station))
                                             .ToListAsync()
                    : await _context.Stations.OrderBy(_orderDict[pageRequest.OrderBy.ToLower()])
                                             .Skip(skippedCount)
                                             .Where(station => (validStatus) ? station.Status == (byte)statusEnum : true)
                                             .Where(station => pageRequest.Code != null ? station.Code.Contains(pageRequest.Code) : true)
                                             .Select(station => _mapper.Map<StationListingDto>(station))
                                             .ToListAsync();
            }
            pageResponse.Data = stations;
            pageResponse.PageSize = (int)pageRequest.PageSize;
            pageResponse.PageCount = (int)(totalCount / pageRequest.PageSize) + 1;
            pageResponse.PageSize = (int)pageRequest.PageSize;
            return pageResponse;
        }

        public async Task<StationDto> Update(int createdById, StationInputDto inputDto, int id)
        {
            Station? station = await _context.Stations
                .Include(station => station.CreatedBy)
                .FirstOrDefaultAsync(station => station.Id == id);
            if (station == null)
            {
                throw new EntityNotFoundException("Station", id);
            }
            else
            {
                if (station.Status == (byte)StationStatusEnum.Deleted)
                {
                    return null;
                }
            }
            await CheckUpdateDuplicate(id, inputDto);
            if (errors.IsNullOrEmpty())
            {
                station = _mapper.Map(inputDto, station);
                if (inputDto.Image != null)
                {
                    if (station.Image != null)
                    {
                        string fileName = station.Image.Substring(station.Image.LastIndexOf('/') + 1).Replace("?alt=media", "").Replace("%2F", "/");
                        await _storageService.DeleteFile(fileName);
                    }
                    Uri uri = await _storageService.UploadFile(inputDto.Code, inputDto.Image, "stations");
                    station.Image = cloudStoragePrefix + uri.AbsolutePath.Substring(uri.AbsolutePath.LastIndexOf('/') + 1) + "?alt=media";
                }
                _context.Stations.Update(station);
                await _context.SaveChangesAsync();
                return _mapper.Map<StationDto>(station);
            }
            else
            {
                throw new DuplicateException(errors);
            }
        }

        private async Task CheckCreateDuplicate(StationInputDto inputDto)
        {
            List<Station> stations = await _context.Stations
                .Where(station => station.Status != (byte)StationStatusEnum.Deleted)
                .Where(station => station.Code == inputDto.Code)
                .ToListAsync();
            foreach (Station station in stations)
            {
                if (!errors.ContainsKey("Code"))
                {
                    errors.Add("Code", "Code is unavailable");
                }
            }
        }

        private async Task CheckUpdateDuplicate(int id, StationInputDto inputDto)
        {
            List<Station> stations = await _context.Stations
                .Where(station => station.Id != id)
                .Where(station => station.Status != (byte)StationStatusEnum.Deleted)
                .Where(station => station.Code == inputDto.Code)
                .ToListAsync();
            foreach (Station station in stations)
            {
                if (!errors.ContainsKey("Code"))
                {
                    errors.Add("Code", "Code is unavailable");
                }
            }
        }
    }
}
