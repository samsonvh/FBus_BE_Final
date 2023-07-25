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
            Trip trip = await _context.Trips.FirstAsync(trip => trip.Id == id && trip.Status == (byte)TripStatusEnum.Deleted);
            if(trip != null)
            {
                switch (trip.Status)
                {
                    case (byte)TripStatusEnum.Active:
                        trip.Status = (byte)TripStatusEnum.Inactive;
                        break;
                    case (byte)TripStatusEnum.Inactive:
                        trip.Status = (byte)TripStatusEnum.Active;
                        break;
                    default:
                        return false;
                }
                _context.Trips.Update(trip);
                await _context.SaveChangesAsync();
                return true;
            } else
            {
                throw new EntityNotFoundException("Trip", id);
            }
        }

        public async Task<TripDto> Create(int createdById, TripInputDto inputDto)
        {
            Trip trip = _mapper.Map<Trip>(inputDto);
            trip.Status = (byte)TripStatusEnum.Active;
            _context.Trips.Add(trip);
            await _context.SaveChangesAsync();
            return _mapper.Map<TripDto>(trip);
        }

        public async Task<bool> Delete(int id)
        {
            Trip trip = await _context.Trips.FirstOrDefaultAsync(trip => trip.Id == id);
            if(trip != null)
            {
                if(trip.Status != (byte) TripStatusEnum.Deleted && trip.Status != (byte) TripStatusEnum.OnGoing && trip.Status != (byte)TripStatusEnum.Finished)
                {
                    trip.Status = (byte) TripStatusEnum.Deleted;
                    _context.Trips.Update(trip);
                    await _context.SaveChangesAsync();
                    return true;
                } else
                {
                    return false;
                }
            } else
            {
                throw new EntityNotFoundException("Trip", id);
            }
        }

        public async Task<TripDto> GetDetails(int id)
        {
            Trip trip = await _context.Trips
                .Include(trip => trip.Driver)
                .Include(trip => trip.Bus)
                .Include(trip => trip.Route)
                .FirstOrDefaultAsync(trip => trip.Id == id && trip.Status != (byte) TripStatusEnum.Deleted);
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
            int skippedCount = (int)((pageRequest.PageIndex - 1) * pageRequest.PageSize);
            List<TripDto> trips = new List<TripDto>();
            int totalCount = await _context.Trips
                .Where(trip => trip.Status != (byte)TripStatusEnum.Deleted)
                .CountAsync();
            if(totalCount > 0)
            {
                trips = await _context.Trips.OrderBy(_orderDict[pageRequest.OrderBy.ToLower()])
                    .Skip(skippedCount)
                    .Where(trip => trip.Status != (byte) TripStatusEnum.Deleted)
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
            Trip trip = await _context.Trips
                .FirstOrDefaultAsync(trip => trip.Id == id && trip.Status != (byte)TripStatusEnum.Deleted);
            if (trip != null)
            {
                if(trip.Status == (byte)TripStatusEnum.Active || trip.Status == (byte)TripStatusEnum.Inactive)
                {
                    trip = _mapper.Map(inputDto, trip);
                    _context.Trips.Update(trip);
                    await _context.SaveChangesAsync();
                    return _mapper.Map<TripDto>(trip);
                } else
                {
                    return null;
                }
            }
            else
            {
                throw new EntityNotFoundException("Trip", id);
            }
        }
    }
}
