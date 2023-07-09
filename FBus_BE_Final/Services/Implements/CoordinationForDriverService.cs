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
    public class CoordinationForDriverService : ICoordinationForDriverService
    {
        private Dictionary<string, string> errors;
        private readonly FbusMainContext _context;
        private readonly IMapper _mapper;
        private readonly Dictionary<string, Expression<Func<Coordination, object>>> _orderDict;

        public CoordinationForDriverService(FbusMainContext context, IMapper mapper)
        {
            errors = new Dictionary<string, string>();
            _context = context;
            _mapper = mapper;
            _orderDict = new Dictionary<string, Expression<Func<Coordination, object>>>
            {
                {"id", coordination => coordination.Id},
                {"dateline", coordination => coordination.DateLine }
            };
        }

        public Task<bool> ChangeStatus(int id, string status)
        {
            throw new NotImplementedException();
        }

        public Task<CoordinationDto> Create(int createdById, CoordinationInputDto inputDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<CoordinationDto> GetDetails(int id, int driverId)
        {
            Coordination? coordination = await _context.Coordinations
                .Include(coor => coor.CreatedBy)
                .Include(coor => coor.Bus)
                .Include(coor => coor.Route).ThenInclude(route => route.CreatedBy)
                .FirstOrDefaultAsync(coor => coor.Id == id);
            if (coordination != null)
            {
                if (coordination.DriverId == driverId)
                {
                    return _mapper.Map<CoordinationDto>(coordination);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new EntityNotFoundException("Coordination", id);
            }
        }

        public async Task<DefaultPageResponse<CoordinationListingDto>> GetList(int driverId, CoordinationPageRequest pageRequest)
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
            List<CoordinationListingDto> coordinations = new List<CoordinationListingDto>();
            int totalCount = await _context.Coordinations
                .Where(coor => coor.Status != (byte)CoordinationStatusEnum.Deleted)
                .Where(coor => coor.DriverId == driverId)
                .CountAsync();
            if (totalCount > 0)
            {
                pageRequest.OrderBy = "dateline";
                pageRequest.Direction = "desc";
                coordinations = pageRequest.Direction == "desc"
                    ? await _context.Coordinations.OrderByDescending(_orderDict[pageRequest.OrderBy.ToLower()])
                                                  .Include(coor => coor.Bus)
                                                  .Include(coor => coor.Driver).ThenInclude(route => route.Account)
                                                  .Include(coor => coor.Route)
                                                  .Where(coor => coor.Status != (byte)CoordinationStatusEnum.Deleted)
                                                  .Where(coor => coor.DriverId == driverId)
                                                  .Select(coor => _mapper.Map<CoordinationListingDto>(coor))
                                                  .ToListAsync()
                    : await _context.Coordinations.OrderBy(_orderDict[pageRequest.OrderBy.ToLower()])
                                                  .Include(coor => coor.Bus)
                                                  .Include(coor => coor.Driver).ThenInclude(route => route.Account)
                                                  .Include(coor => coor.Route)
                                                  .Where(coor => coor.Status != (byte)CoordinationStatusEnum.Deleted)
                                                  .Where(coor => coor.DriverId == driverId)
                                                  .Select(coor => _mapper.Map<CoordinationListingDto>(coor))
                                                  .ToListAsync();
            }
            pageResponse.Data = coordinations;
            pageResponse.PageSize = (int)pageRequest.PageSize;
            pageResponse.PageCount = (int)(totalCount / pageRequest.PageSize) + 1;
            pageResponse.PageSize = (int)pageRequest.PageSize;
            return pageResponse;
        }

        public Task<CoordinationDto> Update(int createdById, CoordinationInputDto inputDto, int id)
        {
            throw new NotImplementedException();
        }
    }
}
