using FBus_BE.DTOs;
using FBus_BE.DTOs.InputDTOs;
using FBus_BE.DTOs.PageDTOs;
using FBus_BE.Enums;
using FBus_BE.Exceptions;
using FBus_BE.Services;
using FBus_BE.Services.Implements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FBus_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripController : ControllerBase, IDefaultController<TripPageRequest, TripInputDto>
    {
        private readonly ITripService _tripService;
        private ITripForDriverService _tripForDriverService;

        public TripController(ITripService tripService, ITripForDriverService tripForDriverService)
        {
            _tripService = tripService;
            _tripForDriverService = tripForDriverService;
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [Authorize("AdminOnly")]
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> ChangeStatus([FromRoute] int id, [FromBody] string status)
        {
            try
            {
                return Ok(await _tripService.ChangeStatus(id, status));
            } catch(EntityNotFoundException entityNotFoundException)
            {
                return BadRequest(new ErrorDto { Title = "Entity Not Found", Errors = new Dictionary<string, string>() { { "message", entityNotFoundException.InforMessage } } });
            }
        }

        [NonAction]
        public Task<IActionResult> CreateWithBody([FromBody] TripInputDto inputDto)
        {
            throw new NotImplementedException();
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TripDto))]
        [Authorize("AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> CreateWithForm([FromForm] TripInputDto inputDto)
        {
            int userId = Convert.ToInt32(User.FindFirst("Id").Value);
            return Ok(await _tripService.Create(userId, inputDto));
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [Authorize("AdminOnly")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                return Ok(await _tripService.Delete(id));
            }
            catch (EntityNotFoundException entityNotFoundException)
            {
                return BadRequest(new ErrorDto { Title = "Entity Not Found", Errors = { { "message", entityNotFoundException.InforMessage } } });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TripDto))]
        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDetails([FromRoute] int id)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst("Id").Value);
                string role = User.FindFirst("Role").Value;
                if (role == nameof(RoleEnum.Driver))
                {
                    return Ok(await _tripForDriverService.GetDetails(id));
                }
                else
                {
                    return Ok(await _tripService.GetDetails(id));
                }
            }
            catch(EntityNotFoundException entityNotFoundException)
            {
                return BadRequest(new ErrorDto { Title = "Entity Not Found", Errors = { { "message", entityNotFoundException.InforMessage } } });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DefaultPageResponse<TripDto>))]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] TripPageRequest pageRequest)
        {
            int userId = Convert.ToInt32(User.FindFirst("Id").Value);
            string role = User.FindFirst("Role").Value;
            if (role == nameof(RoleEnum.Driver))
            {
                return Ok(await _tripForDriverService.GetList(userId, pageRequest));
            }
            else
            {
                return Ok(await _tripService.GetList(pageRequest));
            }
        }

        [NonAction]
        public Task<IActionResult> UpdateWithBody([FromRoute] int id, [FromBody] TripInputDto inputDto)
        {
            throw new NotImplementedException();
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TripDto))]
        [Authorize("AdminOnly")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateWithForm([FromRoute] int id, [FromForm] TripInputDto inputDto)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst("Id").Value);
                return Ok(await _tripService.Update(userId, inputDto, id));
            }
            catch (EntityNotFoundException entityNotFoundException)
            {
                return BadRequest(new ErrorDto { Title = "Entity Not Found", Errors = { { "message", entityNotFoundException.InforMessage } } });
            }
        }
    }
}
