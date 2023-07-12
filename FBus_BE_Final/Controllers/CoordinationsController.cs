﻿using FBus_BE.DTOs;
using FBus_BE.DTOs.InputDTOs;
using FBus_BE.DTOs.ListingDTOs;
using FBus_BE.DTOs.PageDTOs;
using FBus_BE.Enums;
using FBus_BE.Exceptions;
using FBus_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FBus_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoordinationsController : ControllerBase, IDefaultController<CoordinationPageRequest, CoordinationInputDto>
    {
        private readonly ICoordinationService _coordinationService;
        private readonly ICoordinationForDriverService _coordinationForDriverService;

        public CoordinationsController(ICoordinationService coordinationService, ICoordinationForDriverService coordinationForDriverService)
        {
            _coordinationService = coordinationService;
            _coordinationForDriverService = coordinationForDriverService;
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [Authorize("AdminOnly")]
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> ChangeStatus([FromRoute] int id, [FromBody] string status)
        {
            try
            {
                return Ok(await _coordinationService.ChangeStatus(id, status));
            } catch ( EntityNotFoundException entityNotFoundException)
            {
                return BadRequest(new ErrorDto { Title = "Entity Not Found", Errors = entityNotFoundException.InforMessage });
            }
        }

        [NonAction]
        public Task<IActionResult> CreateWithBody([FromBody] CoordinationInputDto inputDto)
        {
            throw new NotImplementedException();
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CoordinationDto))]
        [Authorize("AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> CreateWithForm([FromForm] CoordinationInputDto inputDto)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst("Id").Value);
                return Ok(await _coordinationService.Create(userId, inputDto));
            }
            catch (OccupiedException occupiedException)
            {
                return BadRequest(new ErrorDto { Title="Occupied", Errors= occupiedException.Errors });
            }
            catch (CoordinationDateInvalidException coordinationDateInvalidException)
            {
                return BadRequest(new ErrorDto { Title = "Coordination Date Invalid", Errors = coordinationDateInvalidException.Errors });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [Authorize("AdminOnly")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                return Ok(await _coordinationService.Delete(id));
            }
            catch (EntityNotFoundException entityNotFoundException)
            {
                return BadRequest(new ErrorDto { Title = "Entity Not Found", Errors = entityNotFoundException.InforMessage });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CoordinationDto))]
        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDetails([FromRoute] int id)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst("Id").Value);
                string role = User.FindFirst("Role").Value;
                switch (role)
                {
                    case nameof(RoleEnum.Admin):
                        return Ok(await _coordinationService.GetDetails(id));
                    case nameof(RoleEnum.Driver):
                        return Ok(await _coordinationForDriverService.GetDetails(id, userId));
                    default:
                        return null;
                }
            }
            catch (EntityNotFoundException entityNotFoundException)
            {
                return BadRequest(new ErrorDto { Title = "Entity Not Found", Errors = entityNotFoundException.InforMessage });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DefaultPageResponse<CoordinationListingDto>))]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] CoordinationPageRequest pageRequest)
        {
            int userId = Convert.ToInt32(User.FindFirst("Id").Value);
            string role = User.FindFirst("Role").Value;
            switch (role)
            {
                case nameof(RoleEnum.Admin):
                    return Ok(await _coordinationService.GetList(pageRequest));
                case nameof(RoleEnum.Driver):
                    return Ok(await _coordinationForDriverService.GetList(userId, pageRequest));
                default:
                    return null;
            }
        }

        [NonAction]
        public Task<IActionResult> UpdateWithBody([FromRoute] int id, [FromBody] CoordinationInputDto inputDto)
        {
            throw new NotImplementedException();
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CoordinationDto))]
        [Authorize("AdminOnly")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateWithForm([FromRoute] int id, [FromForm] CoordinationInputDto inputDto)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst("Id").Value);
                return Ok(await _coordinationService.Update(userId, inputDto, id));
            }
            catch (OccupiedException occupiedException)
            {
                return BadRequest(new ErrorDto { Title = "Occupied", Errors = occupiedException.Errors });
            }
            catch (CoordinationDateInvalidException coordinationDateInvalidException)
            {
                return BadRequest(new ErrorDto { Title = "Coordination Date Invalid", Errors = coordinationDateInvalidException.Errors });
            }
        }
    }
}
