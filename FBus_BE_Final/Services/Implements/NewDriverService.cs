using AutoMapper;
using FBus_BE.DTOs;
using FBus_BE.DTOs.InputDTOs;
using FBus_BE.DTOs.ListingDTOs;
using FBus_BE.DTOs.PageDTOs;
using FBus_BE.Models;
using Google.Apis.Storage.v1;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FBus_BE.Services.Implements
{
    public class NewDriverService : INewDriverService
    {
        private Dictionary<string, string> errors;
        private readonly FbusMainContext _context;
        private readonly IMapper _mapper;
        private readonly Dictionary<string, Expression<Func<Driver, object>>> _orderDict;
        private readonly FirebaseStorageService _storageService;
        //private const string cloudStoragePrefix = @"https://firebasestorage.googleapis.com/v0/b/fbus-388009.appspot.com/o/";

        //public NewDriverService(FbusMainContext context, IMapper mapper, IFirebaseStorageService storageService) {
        //    errors = new Dictionary<string, string>();
        //    _context = context;
        //    _mapper = mapper;
        //    _orderDict = new Dictionary<string, Expression<Func<Driver, object>>> {
        //        { "id", driver => driver.Id }
        //    };
        //    _storageService = storageService;
        //}

        public NewDriverService(FbusMainContext context, IMapper mapper)
        {
            errors = new Dictionary<string, string>();
            _context = context;
            _mapper = mapper;
            _orderDict = new Dictionary<string, Expression<Func<Driver, object>>>
            {
                { "id", driver => driver.Id }
            };
            _storageService = new FirebaseStorageService();
        }

        public Task<bool> ChangeStatus(int id, string status)
        {
            throw new NotImplementedException();
        }

        public Task<DriverDto> Create(int createdById, DriverInputDto inputDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<DriverDto> GetDetails(int id)
        {
            throw new NotImplementedException();
        }

        public Task<DefaultPageResponse<DriverListingDto>> GetList(DriverPageRequest pageRequest)
        {
            throw new NotImplementedException();
        }

        public Task<DriverDto> Update(int createdById, DriverInputDto inputDto, int id)
        {
            throw new NotImplementedException();
        }
    }
}
