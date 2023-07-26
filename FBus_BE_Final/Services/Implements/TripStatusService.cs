using AutoMapper;
using FBus_BE.DTOs;
using FBus_BE.DTOs.InputDTOs;
using FBus_BE.DTOs.PageDTOs;
using FBus_BE.Enums;
using FBus_BE.Exceptions;
using FBus_BE.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FBus_BE.Services.Implements
{
    public class TripStatusService : ITripStatusService
    {
        private Dictionary<string, string> errors;
        private readonly FbusMainContext _context;
        private readonly IMapper _mapper;
        private readonly Dictionary<string, Expression<Func<TripStatus, object>>> _orderDict;

        public TripStatusService(FbusMainContext context, IMapper mapper)
        {
            errors = new Dictionary<string, string>();
            _context = context;
            _mapper = mapper;
            _orderDict = new Dictionary<string, Expression<Func<TripStatus, object>>>
            {
                {"id", tripStatus => tripStatus.Id }
            };
        }

        public Task<bool> ChangeStatus(int id, string status)
        {
            throw new NotImplementedException();
        }

        public async Task<TripStatusDto> Create(int createdById, TripStatusInputDto inputDto)
        {
            Trip trip = await _context.Trips
                .Include(trip => trip.Driver)
                .Include(trip => trip.Route).ThenInclude(route => route.RouteStations)
                .FirstOrDefaultAsync(trip => trip.Id == inputDto.TripId);
            if (trip != null)
            {
                if (trip.Driver.AccountId == createdById)
                {
                    int? latestTripStatusOrder = await _context.TripStatuses
                        .OrderBy(tripStatus => tripStatus.Id)
                        .Where(tripStatus => tripStatus.TripId == trip.Id)
                        .Select(tripStatus => tripStatus.StatusOrder)
                        .LastOrDefaultAsync();
                    if (inputDto.CountDown == null)
                    {
                        inputDto.CountDown = 0;
                    }
                    if (inputDto.CountUp == null)
                    {
                        inputDto.CountUp = 0;
                    }
                    TripStatus tripStatus = _mapper.Map<TripStatus>(inputDto);
                    tripStatus.CreatedById = (short?)createdById;
                    if (latestTripStatusOrder == 0)
                    {
                        tripStatus.StatusOrder = 1;
                        tripStatus.Status = (byte)TripStatusEnum.OnGoing;
                        trip.Status = (byte)TripStatusEnum.OnGoing;
                        _context.TripStatuses.Add(tripStatus);
                        _context.Trips.Update(trip);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        tripStatus.StatusOrder = (byte)(latestTripStatusOrder + 1);
                        short lastStationId = (short)trip.Route.RouteStations.Last().StationId;
                        if(lastStationId == inputDto.StationId)
                        {
                            tripStatus.Status = (byte)TripStatusEnum.Finished;
                            trip.Status = (byte)TripStatusEnum.Finished;
                            _context.TripStatuses.Add(tripStatus);
                            _context.Trips.Update(trip);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            tripStatus.Status = (byte)TripStatusEnum.OnGoing;
                            _context.TripStatuses.Add(tripStatus);
                            await _context.SaveChangesAsync();
                        }
                    }
                    return _mapper.Map<TripStatusDto>(tripStatus);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new EntityNotFoundException("Trip", (int)inputDto.TripId);
            }
        }

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<TripStatusDto> GetDetails(int id)
        {
            throw new NotImplementedException();
        }

        public Task<DefaultPageResponse<TripStatusDto>> GetList(TripStatusPageRequest pageRequest)
        {
            throw new NotImplementedException();
        }

        public Task<TripStatusDto> Update(int createdById, TripStatusInputDto inputDto, int id)
        {
            throw new NotImplementedException();
        }
    }
}
