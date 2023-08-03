using AutoMapper;
using FBus_BE.DTOs;
using FBus_BE.DTOs.InputDTOs;
using FBus_BE.DTOs.ListingDTOs;
using FBus_BE.DTOs.PageDTOs;
using FBus_BE.Enums;
using FBus_BE.Exceptions;
using FBus_BE.Models;
using FBus_BE.Utils;
using Google.Apis.Storage.v1;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;

namespace FBus_BE.Services.Implements
{
    public class BusService : IBusService
    {
        private Dictionary<string, string> errors;
        private readonly FbusMainContext _context;
        private readonly IMapper _mapper;
        private readonly Dictionary<string, Expression<Func<Bus, object>>> _orderDict;

        public BusService(FbusMainContext context, IMapper mapper)
        {
            this.errors = new Dictionary<string, string>();
            _context = context;
            _mapper = mapper;
            _orderDict = new Dictionary<string, Expression<Func<Bus, object>>> {
                { "id", bus => bus.Id }
            };
        }

        public async Task<bool> ChangeStatus(int id, string status)
        {
            Bus? bus = await _context.Buses.FirstOrDefaultAsync(bus => bus.Id == id);
            if (bus != null)
            {
                if (bus.Status != (byte)BusStatusEnum.Deleted && bus.Status != (byte)BusStatusEnum.OnGoing)
                {
                    int onGoingTrips = await _context.Trips.Where(trip => trip.BusId == id).CountAsync();
                    status = TextUtil.Capitalize(status);
                    BusStatusEnum statusEnum;
                    switch (status)
                    {
                        case nameof(BusStatusEnum.Active):
                            statusEnum = BusStatusEnum.Active;
                            break;
                        case nameof(BusStatusEnum.Inactive):
                            if (onGoingTrips > 0)
                            {
                                return false;
                            }
                            else
                            {
                                statusEnum = BusStatusEnum.Inactive;
                                break;
                            }
                        default:
                            return false;
                    }
                    bus.Status = (byte)statusEnum;
                    _context.Buses.Update(bus);
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
                throw new EntityNotFoundException("Bus", id);
            }
        }

        public async Task<BusDto> Create(int createdById, BusInputDto inputDto)
        {
            await CheckCreateDuplicate(inputDto);
            if (errors.IsNullOrEmpty())
            {
                Bus bus = _mapper.Map<Bus>(inputDto);
                bus.Code = await CreateCode();
                bus.CreatedById = (short?)createdById;
                bus.Status = (byte)BusStatusEnum.Active;
                _context.Buses.Add(bus);
                await _context.SaveChangesAsync();

                return _mapper.Map<BusDto>(bus);
            }
            else
            {
                throw new DuplicateException(errors);
            }
        }

        public async Task<bool> Delete(int id)
        {
            Bus? bus = await _context.Buses.FirstOrDefaultAsync(bus => bus.Id == id);
            if (bus != null)
            {
                if (bus.Status != (byte)BusStatusEnum.Deleted && bus.Status != (byte)BusStatusEnum.OnGoing)
                {
                    int onGoingTrips = await _context.Trips.Where(trip => trip.BusId == id).CountAsync();
                    if (onGoingTrips > 0)
                    {
                        return false;
                    }
                    else
                    {
                        bus.Status = (byte)BusStatusEnum.Deleted;
                        _context.Buses.Update(bus);

                        List<Trip> trips = await _context.Trips.Where(trip => trip.BusId == id && trip.Status == (byte)TripStatusEnum.Active).ToListAsync();
                        foreach (Trip trip in trips)
                        {
                            trip.Status = (byte)TripStatusEnum.Inactive;
                            _context.Trips.Update(trip);
                        }

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
                throw new EntityNotFoundException("Bus", id);
            }
        }

        public async Task<BusDto> GetDetails(int id)
        {
            Bus? bus = await _context.Buses.Include(bus => bus.CreatedBy).FirstOrDefaultAsync(bus => bus.Id == id);
            if (bus != null)
            {
                return _mapper.Map<BusDto>(bus);
            }
            else
            {
                throw new EntityNotFoundException("Bus", id);
            }
        }

        public async Task<DefaultPageResponse<BusListingDto>> GetList(BusPageRequest pageRequest)
        {
            DefaultPageResponse<BusListingDto> pageResponse = new DefaultPageResponse<BusListingDto>();
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
            BusStatusEnum statusEnum = BusStatusEnum.Active;
            bool validStatus = false;
            if (pageRequest.Status != null)
            {
                switch (TextUtil.Capitalize(pageRequest.Status))
                {
                    case nameof(BusStatusEnum.Active):
                        statusEnum = BusStatusEnum.Active;
                        validStatus = true;
                        break;
                    case nameof(BusStatusEnum.Inactive):
                        statusEnum = BusStatusEnum.Inactive;
                        validStatus = true;
                        break;
                    case nameof(BusStatusEnum.Deleted):
                        statusEnum = BusStatusEnum.Deleted;
                        validStatus = true;
                        break;
                }
            }
            int skippedCount = (int)((pageRequest.PageIndex - 1) * pageRequest.PageSize);
            List<BusListingDto> buses = new List<BusListingDto>();
            int totalCount = await _context.Buses
                .Where(bus => (validStatus) ? bus.Status == (byte)statusEnum : true)
                .Where(bus => (pageRequest.Code != null && pageRequest.LicensePlate != null)
                               ? bus.Code.Contains(pageRequest.Code) || bus.LicensePlate.Contains(pageRequest.LicensePlate)
                               : (pageRequest.Code != null && pageRequest.LicensePlate == null)
                                  ? bus.Code.Contains(pageRequest.Code)
                                  : (pageRequest.Code == null && pageRequest.LicensePlate != null)
                                     ? bus.LicensePlate.Contains(pageRequest.LicensePlate)
                                     : true)
                .CountAsync();
            if (totalCount > 0)
            {
                buses = pageRequest.Direction == "desc"
                    ? await _context.Buses.OrderByDescending(_orderDict[pageRequest.OrderBy.ToLower()])
                                          .Skip(skippedCount)
                                          .Where(bus => (validStatus) ? bus.Status == (byte)statusEnum : true)
                                          .Where(bus => (pageRequest.Code != null && pageRequest.LicensePlate != null)
                                                         ? bus.Code.Contains(pageRequest.Code) || bus.LicensePlate.Contains(pageRequest.LicensePlate)
                                                         : (pageRequest.Code != null && pageRequest.LicensePlate == null)
                                                            ? bus.Code.Contains(pageRequest.Code)
                                                            : (pageRequest.Code == null && pageRequest.LicensePlate != null)
                                                               ? bus.LicensePlate.Contains(pageRequest.LicensePlate)
                                                               : true)
                                          .Select(bus => _mapper.Map<BusListingDto>(bus))
                                          .ToListAsync()
                    : await _context.Buses.OrderBy(_orderDict[pageRequest.OrderBy.ToLower()])
                                          .Skip(skippedCount)
                                          .Where(bus => (validStatus) ? bus.Status == (byte)statusEnum : true)
                                          .Where(bus => (pageRequest.Code != null && pageRequest.LicensePlate != null)
                                                         ? bus.Code.Contains(pageRequest.Code) || bus.LicensePlate.Contains(pageRequest.LicensePlate)
                                                         : (pageRequest.Code != null && pageRequest.LicensePlate == null)
                                                            ? bus.Code.Contains(pageRequest.Code)
                                                            : (pageRequest.Code == null && pageRequest.LicensePlate != null)
                                                               ? bus.LicensePlate.Contains(pageRequest.LicensePlate)
                                                               : true)
                                          .Select(bus => _mapper.Map<BusListingDto>(bus))
                                          .ToListAsync();
            }
            pageResponse.Data = buses;
            pageResponse.PageSize = (int)pageRequest.PageSize;
            pageResponse.PageCount = (int)(totalCount / pageRequest.PageSize) + 1;
            pageResponse.PageSize = (int)pageRequest.PageSize;
            return pageResponse;
        }

        public async Task<BusDto> Update(int createdById, BusInputDto inputDto, int id)
        {
            Bus? bus = await _context.Buses.Include(bus => bus.CreatedBy).FirstOrDefaultAsync(bus => bus.Id == id);
            if (bus == null)
            {
                throw new EntityNotFoundException("Bus", id);
            }
            await CheckUpdateDuplicate(id, inputDto);
            if (errors.IsNullOrEmpty())
            {
                bus = _mapper.Map(inputDto, bus);
                _context.Buses.Update(bus);
                await _context.SaveChangesAsync();

                return _mapper.Map<BusDto>(bus);
            }
            else
            {
                throw new DuplicateException(errors);
            }
        }

        private async Task CheckCreateDuplicate(BusInputDto inputDto)
        {
            List<Bus> buses = await _context.Buses
                .Where(bus => bus.LicensePlate == inputDto.LicensePlate)
                .Select(bus => new Bus { Code = bus.Code, LicensePlate = bus.LicensePlate })
                .ToListAsync();
            foreach (Bus bus in buses)
            {
                //if (bus.Code == inputDto.Code)
                //{
                //    if (!errors.ContainsKey("Code"))
                //    {
                //        errors.Add("Code", "Code is unavailable");
                //    }
                //}
                if (bus.LicensePlate == inputDto.LicensePlate)
                {
                    if (!errors.ContainsKey("LicensePlate"))
                    {
                        errors.Add("LicensePlate", "LicensePlate is used by " + bus.Code);
                    }
                }
                if (errors.Count == 2)
                {
                    break;
                }
            }
        }

        private async Task CheckUpdateDuplicate(int id, BusInputDto inputDto)
        {
            List<Bus> buses = await _context.Buses
                .Where(bus => bus.Id != id)
                .Where(bus => bus.LicensePlate == inputDto.LicensePlate)
                .Select(bus => new Bus { Code = bus.Code, LicensePlate = bus.LicensePlate })
                .ToListAsync();
            foreach (Bus bus in buses)
            {
                //if (bus.Code == inputDto.Code)
                //{
                //    if (!errors.ContainsKey("Code"))
                //    {
                //        errors.Add("Code", "Code is unavailable");
                //    }
                //}
                if (bus.LicensePlate == inputDto.LicensePlate)
                {
                    if (!errors.ContainsKey("LicensePlate"))
                    {
                        errors.Add("LicensePlate", "LicensePlate is used by " + bus.Code);
                    }
                }
                if (errors.Count == 2)
                {
                    break;
                }
            }
        }

        private async Task<string> CreateCode()
        {
            string code = "FBUS";
            int countCode = await _context.Buses.CountAsync();
            countCode += 1;
            code += countCode;
            return code;
        }
    }
}
