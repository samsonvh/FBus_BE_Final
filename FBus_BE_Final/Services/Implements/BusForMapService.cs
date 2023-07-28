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
    public class BusForMapService : IBusForMapService
    {
        private Dictionary<string, string> errors;
        private readonly FbusMainContext _context;
        private readonly IMapper _mapper;
        private readonly Dictionary<string, Expression<Func<Bus, object>>> _orderDict;

        public BusForMapService(FbusMainContext context, IMapper mapper)
        {
            this.errors = new Dictionary<string, string>();
            _context = context;
            _mapper = mapper;
            _orderDict = new Dictionary<string, Expression<Func<Bus, object>>> {
                { "id", bus => bus.Id }
            };
        }
        public Task<bool> ChangeStatus(int id, string status)
        {
            throw new NotImplementedException();
        }

        public Task<BusDto> Create(int createdById, BusInputDto inputDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<BusDto> GetDetails(int id)
        {
            Bus? bus = await _context.Buses
                .FirstOrDefaultAsync(bus => bus.Id == id && bus.Status == (byte)BusStatusEnum.Active);
            if (bus != null)
            {
                return _mapper.Map<BusDto>(bus);
            }
            else
            {
                throw new EntityNotFoundException("Bus", id);
            }
        }

        public Task<DefaultPageResponse<BusListingDto>> GetList(BusPageRequest pageRequest)
        {
            throw new NotImplementedException();
        }

        public Task<BusDto> Update(int createdById, BusInputDto inputDto, int id)
        {
            throw new NotImplementedException();
        }
    }
}
