﻿using AutoMapper;
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
    public class TripForDriverService : ITripForDriverService
    {
        private Dictionary<string, string> errors;
        private readonly FbusMainContext _context;
        private readonly IMapper _mapper;
        private readonly Dictionary<string, Expression<Func<Trip, object>>> _orderDict;

        public TripForDriverService(FbusMainContext context, IMapper mapper)
        {
            errors = new Dictionary<string, string>();
            _context = context;
            _mapper = mapper;
            _orderDict = new Dictionary<string, Expression<Func<Trip, object>>>
            {
                {"id", trip => trip.Id }
            };
        }

        public Task<bool> ChangeStatus(int id, string status)
        {
            throw new NotImplementedException();
        }

        public Task<TripDto> Create(int createdById, TripInputDto inputDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<TripDto> GetDetails(int id)
        {
            Trip trip = await _context.Trips
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
            throw new NotImplementedException();
        }

        public Task<TripDto> Update(int createdById, TripInputDto inputDto, int id)
        {
            throw new NotImplementedException();
        }

        public async Task<DefaultPageResponse<TripDto>> GetList(int driverId, TripPageRequest pageRequest)
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
                .Where(trip => trip.DriverId == driverId)
                .CountAsync();
            if (totalCount > 0)
            {
                trips = await _context.Trips.OrderBy(_orderDict[pageRequest.OrderBy.ToLower()])
                    .Skip(skippedCount)
                    .Where(trip => trip.Status != (byte)TripStatusEnum.Deleted)
                    .Where(trip => trip.DriverId == driverId)
                    .Select(trip => _mapper.Map<TripDto>(trip))
                    .ToListAsync();
            }
            pageResponse.Data = trips;
            pageResponse.PageSize = (int)pageRequest.PageSize;
            pageResponse.PageCount = (int)(totalCount / pageRequest.PageSize) + 1;
            pageResponse.PageSize = (int)pageRequest.PageSize;
            return pageResponse;
        }
    }
}