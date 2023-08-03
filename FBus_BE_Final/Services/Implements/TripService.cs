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

namespace FBus_BE.Services.Implements
{
    public class TripService : ITripService
    {
        private Dictionary<string, string> errors;
        private readonly FbusMainContext _context;
        private readonly IMapper _mapper;
        private readonly Dictionary<string, Expression<Func<Trip, object>>> _orderDict;

        public TripService(FbusMainContext context, IMapper mapper)
        {
            errors = new Dictionary<string, string>();
            _context = context;
            _mapper = mapper;
            _orderDict = new Dictionary<string, Expression<Func<Trip, object>>>
            {
                {"id", trip => trip.Id }
            };
        }

        public async Task<bool> ChangeStatus(int id, string status)
        {
            Trip trip = await _context.Trips
                .Include(trip => trip.Route)
                .Include(trip => trip.Bus)
                .Include(trip => trip.Driver)
                .FirstOrDefaultAsync(trip => trip.Id == id && trip.Status != (byte)TripStatusEnum.Deleted);
            if (trip != null)
            {
                if (trip.Status != (byte)TripStatusEnum.Deleted && trip.Status != (byte)TripStatusEnum.OnGoing && trip.Status != (byte)TripStatusEnum.Finished)
                {

                    status = TextUtil.Capitalize(status);
                    TripStatusEnum tripStatusEnum;
                    switch (status)
                    {
                        case nameof(TripStatusEnum.Active):
                            if (trip.Route.Status == (byte)RouteStatusEnum.Deleted
                                || trip.Bus.Status == (byte)RouteStatusEnum.Deleted
                                || trip.Driver.Status == (byte)DriverStatusEnum.Deleted
                                || trip.Route.Status == (byte)DriverStatusEnum.Inactive
                                || trip.Bus.Status == (byte)DriverStatusEnum.Inactive
                                || trip.Driver.Status == (byte)DriverStatusEnum.Inactive)
                            {
                                return false;
                            }
                            else
                            {
                                tripStatusEnum = TripStatusEnum.Active;
                                break;
                            }
                        case nameof(TripStatusEnum.Inactive):
                            tripStatusEnum = TripStatusEnum.Inactive;
                            break;
                        default:
                            return false;
                    }
                    trip.Status = (byte)tripStatusEnum;
                    _context.Trips.Update(trip);
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
                throw new EntityNotFoundException("Trip", id);
            }
        }

        public async Task<TripDto> Create(int createdById, TripInputDto inputDto)
        {
            ValidateTripDate(inputDto.DateLine, inputDto.DueDate);
            await ValidateComponents(inputDto.DriverId, inputDto.BusId, inputDto.RouteId, inputDto.DateLine, inputDto.DueDate);
            Trip trip = _mapper.Map<Trip>(inputDto);
            trip.Status = (byte)TripStatusEnum.Active;
            trip.CreatedById = (short?)createdById;
            _context.Trips.Add(trip);
            await _context.SaveChangesAsync();
            return _mapper.Map<TripDto>(trip);
        }

        public async Task<bool> Delete(int id)
        {
            Trip? trip = await _context.Trips.FirstOrDefaultAsync(trip => trip.Id == id && trip.Status != (byte)TripStatusEnum.Deleted);
            if (trip != null)
            {
                if (trip.Status != (byte)TripStatusEnum.OnGoing && trip.Status != (byte)TripStatusEnum.Finished)
                {
                    trip.Status = (byte)TripStatusEnum.Deleted;
                    _context.Trips.Update(trip);
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
                throw new EntityNotFoundException("Trip", id);
            }
        }

        public async Task<TripDto> GetDetails(int id)
        {
            Trip? trip = await _context.Trips
                .Include(trip => trip.Driver)
                .Include(trip => trip.Bus)
                .Include(trip => trip.Route)
                .FirstOrDefaultAsync(trip => trip.Id == id && trip.Status != (byte)TripStatusEnum.Deleted);
            if (trip != null)
            {
                return _mapper.Map<TripDto>(trip);
            }
            else
            {
                throw new EntityNotFoundException("Trip", id);
            }
        }

        public async Task<DefaultPageResponse<TripDto>> GetList(TripPageRequest pageRequest)
        {
            DefaultPageResponse<TripDto> pageResponse = new DefaultPageResponse<TripDto>();
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
            TripStatusEnum statusEnum = TripStatusEnum.Active;
            bool validStatus = false;
            if (pageRequest.Status != null)
            {
                switch (TextUtil.Capitalize(pageRequest.Status))
                {
                    case nameof(TripStatusEnum.Active):
                        statusEnum = TripStatusEnum.Active;
                        validStatus = true;
                        break;
                    case nameof(TripStatusEnum.Inactive):
                        statusEnum = TripStatusEnum.Inactive;
                        validStatus = true;
                        break;
                    case nameof(TripStatusEnum.Deleted):
                        statusEnum = TripStatusEnum.Deleted;
                        validStatus = true;
                        break;
                    case "Ongoing":
                        statusEnum = TripStatusEnum.OnGoing;
                        validStatus = true;
                        break;
                    case nameof(TripStatusEnum.Finished):
                        statusEnum = TripStatusEnum.Finished;
                        validStatus = true;
                        break;
                }
            }
            int skippedCount = (int)((pageRequest.PageIndex - 1) * pageRequest.PageSize);
            List<TripDto> trips = new List<TripDto>();
            int totalCount = await _context.Trips
                .Where(trip => (validStatus) ? trip.Status == (byte)statusEnum : true)
                .CountAsync();
            if (totalCount > 0)
            {
                trips = await _context.Trips.OrderBy(_orderDict[pageRequest.OrderBy.ToLower()])
                    .Skip(skippedCount)
                    .Include(trip => trip.Driver).ThenInclude(driver => driver.CreatedBy)
                    .Include(trip => trip.Driver).ThenInclude(driver => driver.Account)
                    .Include(trip => trip.Bus).ThenInclude(bus => bus.CreatedBy)
                    .Include(trip => trip.Route).ThenInclude(route => route.CreatedBy)
                    .Include(trip => trip.CreatedBy)
                    .Where(station => (validStatus) ? station.Status == (byte)statusEnum : true)
                    .Select(trip => _mapper.Map<TripDto>(trip))
                    .ToListAsync();
            }
            pageResponse.Data = trips;
            pageResponse.PageSize = (int)pageRequest.PageSize;
            pageResponse.PageCount = (int)(totalCount / pageRequest.PageSize) + 1;
            pageResponse.PageSize = (int)pageRequest.PageSize;
            return pageResponse;
        }

        public async Task<TripDto> Update(int createdById, TripInputDto inputDto, int id)
        {
            ValidateTripDate(inputDto.DateLine, inputDto.DueDate);
            await ValidateComponents(inputDto.DriverId, inputDto.BusId, inputDto.RouteId, inputDto.DateLine, inputDto.DueDate);
            Trip? trip = await _context.Trips
                .FirstOrDefaultAsync(trip => trip.Id == id && trip.Status != (byte)TripStatusEnum.Deleted);
            if (trip != null)
            {
                if (trip.Status == (byte)TripStatusEnum.Active || trip.Status == (byte)TripStatusEnum.Inactive)
                {
                    ValidateTripDate(inputDto.DateLine, inputDto.DueDate);
                    trip = _mapper.Map(inputDto, trip);
                    _context.Trips.Update(trip);
                    await _context.SaveChangesAsync();
                    return _mapper.Map<TripDto>(trip);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new EntityNotFoundException("Trip", id);
            }
        }

        private void ValidateTripDate(DateTime dateLine, DateTime dueDate)
        {
            bool hasErrors = false;
            if (dateLine.Date <= DateTime.Now)
            {
                hasErrors = true;
                errors.Add("dateLine", "DateLine must be beyond today");
            }
            if (dueDate.Date <= DateTime.Now)
            {
                hasErrors = true;
                errors.Add("dueDate", "DueDate must be beyond today");
            }
            else
            {
                if (dueDate.TimeOfDay <= dateLine.TimeOfDay)
                {
                    hasErrors = true;
                    errors.Add("dueDate", "DueDate must be beyond Dateline");
                }
            }
            if (hasErrors)
            {
                throw new TripDateInvalidException(errors);
            }
        }

        private async Task ValidateComponents(short driverId, short busId, short routeId, DateTime dateLine, DateTime dueDate)
        {
            bool hasErrors = false;
            Trip? tripHasDriver = await _context.Trips
                .Where(trip => trip.DriverId == driverId && trip.DateLine >= dateLine && trip.DueDate <= dueDate)
                .FirstOrDefaultAsync();
            if (tripHasDriver != null)
            {
                errors.Add("driverId", "This Driver is occupied within that range of time");
                hasErrors = true;
            }
            Trip? tripHasBus = await _context.Trips
                .Where(trip => trip.BusId == busId && trip.DateLine >= dateLine && trip.DueDate <= dueDate)
                .FirstOrDefaultAsync();
            if (tripHasBus != null)
            {
                errors.Add("busId", "This Bus is occupied within that range of time");
                hasErrors = true;
            }
            Trip? tripHasRoute = await _context.Trips
                .Where(trip => trip.RouteId == routeId && trip.DateLine >= dateLine && trip.DueDate <= dueDate)
                .FirstOrDefaultAsync();
            if (tripHasDriver != null)
            {
                errors.Add("routeId", "This Route is occupied within that range of time");
                hasErrors = true;
            }

            if (hasErrors)
            {
                throw new OccupiedException(errors);
            }
        }
    }
}
