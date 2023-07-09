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
using System.Linq.Expressions;

namespace FBus_BE.Services.Implements
{
    public class CoordinationService : ICoordinationService
    {
        private Dictionary<string, string> errors;
        private readonly FbusMainContext _context;
        private readonly IMapper _mapper;
        private readonly Dictionary<string, Expression<Func<Coordination, object>>> _orderDict;

        public CoordinationService(FbusMainContext context, IMapper mapper)
        {
            errors = new Dictionary<string, string>();
            _context = context;
            _mapper = mapper;
            _orderDict = new Dictionary<string, Expression<Func<Coordination, object>>>
            {
                {"id", coordination => coordination.Id}
            };
        }

        public async Task<bool> ChangeStatus(int id, string status)
        {
            Coordination? coordination = await _context.Coordinations.FirstOrDefaultAsync(coordination => coordination.Id == id);
            if (coordination != null)
            {
                if (coordination.Status == (byte)CoordinationStatusEnum.Active || coordination.Status == (byte)CoordinationStatusEnum.Inactive)
                {
                    status = TextUtil.Capitalize(status);
                    CoordinationStatusEnum statusEnum;
                    switch (status)
                    {
                        case nameof(CoordinationStatusEnum.Active):
                            statusEnum = CoordinationStatusEnum.Active;
                            break;
                        case nameof(CoordinationStatusEnum.Inactive):
                            statusEnum = CoordinationStatusEnum.Inactive;
                            break;
                        default:
                            return false;
                    }
                    coordination.Status = (byte)statusEnum;
                    _context.Coordinations.Update(coordination);
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
                throw new EntityNotFoundException("Coordination", id);
            }
        }

        public async Task<CoordinationDto> Create(int createdById, CoordinationInputDto inputDto)
        {
            if (inputDto.DateLine <= DateTime.Today)
            {
                errors.Add("DateLine", "DateLine must be in future");
            }
            if (inputDto.DueDate < inputDto.DateLine)
            {
                errors.Add("DueDate", "DueDate must be set after DateLine");
            }
            else
            {
                if (inputDto.DueDate == inputDto.DateLine)
                {
                    if (inputDto.DueDate.Hour <= inputDto.DateLine.Hour)
                    {
                        errors.Add("DueDate", "DateLine must be set after DateLine");
                    }
                }
            }
            if (errors.IsNullOrEmpty())
            {
                List<Coordination> coordinations = await _context.Coordinations
                    .Where(coor => coor.Status != (byte)CoordinationStatusEnum.Deleted)
                    .Where(coor => coor.DriverId == inputDto.DriverId || coor.BusId == inputDto.BusId)
                    .Where(coor => coor.DateLine >= inputDto.DateLine && coor.DueDate <= inputDto.DueDate)
                    .ToListAsync();
                if (coordinations.Count > 0)
                {
                    foreach (Coordination coor in coordinations)
                    {
                        if (coor.DriverId == inputDto.DriverId)
                        {
                            errors.Add("DriverId", "This driver is occupied in duration from " + coor.DateLine.ToString() + " to " + coor.DueDate.ToString());
                        }
                        if (coor.BusId == inputDto.BusId)
                        {
                            errors.Add("BusId", "This bus is occupied in duration from " + coor.DateLine.ToString() + " to " + coor.DueDate.ToString());
                        }
                    }
                    throw new OccupiedException(errors);
                }
                Coordination coordination = _mapper.Map<Coordination>(inputDto);
                coordination.CreatedById = (short?)createdById;
                coordination.Status = (byte)CoordinationStatusEnum.Active;
                _context.Coordinations.Add(coordination);
                await _context.SaveChangesAsync();

                CoordinationStatus coordinationStatus = new CoordinationStatus
                {
                    CoordinationId = coordination.Id,
                    OriginalStatus = coordination.Status,
                    UpdatedStatus = coordination.Status,
                    CreatedById = (short?)createdById,
                    Note = "Created",
                    StatusOrder = 1
                };
                _context.CoordinationStatuses.Add(coordinationStatus);
                await _context.SaveChangesAsync();

                return _mapper.Map<CoordinationDto>(coordination);
            }
            else
            {
                throw new CoordinationDateInvalidException(errors);
            }
        }

        public async Task<bool> Delete(int id)
        {
            Coordination? coordination = await _context.Coordinations.FirstOrDefaultAsync(x => x.Id == id);
            if (coordination != null)
            {
                if (coordination.Status != (byte)CoordinationStatusEnum.Deleted && 
                    coordination.Status != (byte)CoordinationStatusEnum.OnGoing && 
                    coordination.Status != (byte)CoordinationStatusEnum.Finished)
                {
                    coordination.Status = (byte)CoordinationStatusEnum.Deleted;
                    CoordinationStatus latestStatus = await _context.CoordinationStatuses
                        .OrderBy(coorStatus => coorStatus.CreatedDate)
                        .LastOrDefaultAsync(coordinationStatus => coordinationStatus.CoordinationId == id);
                    CoordinationStatus coordinationStatus = new CoordinationStatus
                    {
                        CoordinationId = coordination.Id,
                        OriginalStatus = latestStatus.UpdatedStatus,
                        UpdatedStatus = (byte)CoordinationStatusEnum.Deleted,
                        CreatedById = latestStatus.CreatedById,
                        Note = "Deleted",
                        StatusOrder = (byte)(latestStatus.StatusOrder + 1)
                    };
                    _context.CoordinationStatuses.Add(coordinationStatus);
                    _context.Coordinations.Update(coordination);
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
                throw new EntityNotFoundException("Coordination", id);
            }
        }

        public async Task<CoordinationDto> GetDetails(int id)
        {
            Coordination? coordination = await _context.Coordinations
                .Include(coor => coor.CreatedBy)
                .Include(coor => coor.Bus)
                .Include(coor => coor.Route)
                .Include(coor => coor.Driver).ThenInclude(route => route.CreatedBy)
                .Include(coor => coor.Driver).ThenInclude(route => route.Account)
                .FirstOrDefaultAsync(coor => coor.Id == id);
            if(coordination != null)
            {
                return _mapper.Map<CoordinationDto>(coordination);
            }
            else
            {
                throw new EntityNotFoundException("Coordination", id);
            }
        }

        public async Task<DefaultPageResponse<CoordinationListingDto>> GetList(CoordinationPageRequest pageRequest)
        {
            DefaultPageResponse<CoordinationListingDto> pageResponse = new DefaultPageResponse<CoordinationListingDto>();
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
            List<CoordinationListingDto> coordinations = new List<CoordinationListingDto> ();
            int totalCount = await _context.Coordinations
                .Where(coor => coor.Status != (byte)CoordinationStatusEnum.Deleted)
                .CountAsync();
            if(totalCount > 0)
            {
                coordinations = pageRequest.Direction == "desc"
                    ? await _context.Coordinations.OrderByDescending(_orderDict[pageRequest.OrderBy.ToLower()])
                                                  .Skip(skippedCount)
                                                  .Include(coor => coor.Bus)
                                                  .Include(coor => coor.Driver).ThenInclude(route => route.Account)
                                                  .Include(coor => coor.Route)
                                                  .Where(coor => coor.Status != (byte)CoordinationStatusEnum.Deleted)
                                                  .Select(coor => _mapper.Map<CoordinationListingDto>(coor))
                                                  .ToListAsync()
                    : await _context.Coordinations.OrderBy(_orderDict[pageRequest.OrderBy.ToLower()])
                                                  .Skip(skippedCount)
                                                  .Include(coor => coor.Bus)
                                                  .Include(coor => coor.Driver).ThenInclude(route => route.Account)
                                                  .Include(coor => coor.Route)
                                                  .Where(coor => coor.Status != (byte)CoordinationStatusEnum.Deleted)
                                                  .Select(coor => _mapper.Map<CoordinationListingDto>(coor))
                                                  .ToListAsync();
            }
            pageResponse.Data = coordinations;
            pageResponse.PageSize = (int)pageRequest.PageSize;
            pageResponse.PageCount = (int)(totalCount / pageRequest.PageSize) + 1;
            pageResponse.PageSize = (int)pageRequest.PageSize;
            return pageResponse;
        }

        public async Task<CoordinationDto> Update(int createdById, CoordinationInputDto inputDto, int id)
        {
            Coordination? coordination = await _context.Coordinations.FirstOrDefaultAsync(coor => coor.Id == id);
            if(coordination != null)
            {
                if (inputDto.DateLine <= DateTime.Today)
                {
                    errors.Add("DateLine", "DateLine must be in future");
                }
                if (inputDto.DueDate < inputDto.DateLine)
                {
                    errors.Add("DueDate", "DueDate must be set after DateLine");
                }
                else
                {
                    if (inputDto.DueDate == inputDto.DateLine)
                    {
                        if (inputDto.DueDate.Hour <= inputDto.DateLine.Hour)
                        {
                            errors.Add("DueDate", "DateLine must be set after DateLine");
                        }
                    }
                }
                if (errors.IsNullOrEmpty())
                {
                    List<Coordination> coordinations = await _context.Coordinations
                        .Where(coor => coor.Status != (byte)CoordinationStatusEnum.Deleted)
                        .Where(coor => coor.Id != id)
                        .Where(coor => coor.DriverId == inputDto.DriverId || coor.BusId == inputDto.BusId)
                        .Where(coor => coor.DateLine >= inputDto.DateLine && coor.DueDate <= inputDto.DueDate)
                        .ToListAsync();
                    if (coordinations.Count > 0)
                    {
                        foreach (Coordination coor in coordinations)
                        {
                            if (coor.DriverId == inputDto.DriverId)
                            {
                                errors.Add("DriverId", "This driver is occupied in duration from " + coor.DateLine.ToString() + " to " + coor.DueDate.ToString());
                            }
                            if (coor.BusId == inputDto.BusId)
                            {
                                errors.Add("BusId", "This bus is occupied in duration from " + coor.DateLine.ToString() + " to " + coor.DueDate.ToString());
                            }
                        }
                        throw new OccupiedException(errors);
                    }
                    coordination = _mapper.Map(inputDto, coordination);
                    _context.Coordinations.Update(coordination);
                    await _context.SaveChangesAsync();

                    CoordinationStatus latestStatus = await _context.CoordinationStatuses.LastOrDefaultAsync(coordinationStatus => coordinationStatus.CoordinationId == id);
                    CoordinationStatus coordinationStatus = new CoordinationStatus
                    {
                        CoordinationId = coordination.Id,
                        OriginalStatus = latestStatus.UpdatedStatus,
                        UpdatedStatus = latestStatus.UpdatedStatus,
                        CreatedById = (short?)createdById,
                        Note = "Updated",
                        StatusOrder = (byte)(latestStatus.StatusOrder + 1)
                    };
                    _context.CoordinationStatuses.Add(coordinationStatus);
                    await _context.SaveChangesAsync();

                    return _mapper.Map<CoordinationDto>(coordination);
                }
                else
                {
                    throw new CoordinationDateInvalidException(errors);
                }
            }
            else
            {
                throw new EntityNotFoundException("Coordination", id);
            }
        }
    }
}
