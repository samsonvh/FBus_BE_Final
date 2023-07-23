using FBus_BE.DTOs;
using FBus_BE.DTOs.InputDTOs;
using FBus_BE.DTOs.ListingDTOs;
using FBus_BE.DTOs.PageDTOs;
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
    public class StationsController : ControllerBase, IDefaultController<StationPageRequest, StationInputDto>
    {
        private readonly IStationService _stationService;

        public StationsController(IStationService stationService)
        {
            _stationService = stationService;
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [Authorize("AdminOnly")]
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> ChangeStatus([FromRoute] int id, [FromBody] string status)
        {
            try
            {
                return Ok(await _stationService.ChangeStatus(id, status));
            }
            catch (EntityNotFoundException entityNotFoundException)
            {
                return BadRequest(new ErrorDto { Title = "Entity Not Found", Errors = new Dictionary<string, string>() { { "message", entityNotFoundException.InforMessage } } });
            }
        }

        [NonAction]
        public Task<IActionResult> CreateWithBody([FromBody] StationInputDto inputDto)
        {
            throw new NotImplementedException();
        }

        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(StationDto))]
        [Authorize("AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> CreateWithForm([FromForm] StationInputDto inputDto)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst("Id").Value);
                return Ok(await _stationService.Create(userId, inputDto));
            }
            catch (DuplicateException duplicateException)
            {
                return BadRequest(new ErrorDto { Title = "Duplicated", Errors = duplicateException.GetErrors() });
            }
            catch (EntityNotFoundException entityNotFoundException)
            {
                return BadRequest(new ErrorDto { Title = "Entity Not Found", Errors = new Dictionary<string, string>() { { "message", entityNotFoundException.InforMessage } } });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [Authorize("AdminOnly")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                return Ok(await _stationService.Delete(id));
            }
            catch (EntityNotFoundException entityNotFoundException)
            {
                return BadRequest(new ErrorDto { Title = "Entity Not Found", Errors = new Dictionary<string, string>() { { "message", entityNotFoundException.InforMessage } } });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StationDto))]
        [Authorize("AdminOnly")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDetails([FromRoute] int id)
        {
            try
            {
                return Ok(await _stationService.GetDetails(id));
            }
            catch (EntityNotFoundException entityNotFoundException)
            {
                return BadRequest(new ErrorDto { Title = "Entity Not Found", Errors = new Dictionary<string, string>() { { "message", entityNotFoundException.InforMessage } } });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DefaultPageResponse<StationListingDto>))]
        [Authorize("AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] StationPageRequest pageRequest)
        {
            return Ok(await _stationService.GetList(pageRequest));
        }

        [NonAction]
        public Task<IActionResult> UpdateWithBody([FromRoute] int id, [FromBody] StationInputDto inputDto)
        {
            throw new NotImplementedException();
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StationDto))]
        [Authorize("AdminOnly")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateWithForm([FromRoute] int id, [FromForm] StationInputDto inputDto)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst("Id").Value);
                return Ok(await _stationService.Update(userId, inputDto, id));
            }
            catch (DuplicateException duplicateException)
            {
                return BadRequest(new ErrorDto { Title = "Duplicated", Errors = duplicateException.GetErrors() });
            }
            catch (EntityNotFoundException entityNotFoundException)
            {
                return BadRequest(new ErrorDto { Title = "Entity Not Found", Errors = new Dictionary<string, string>() { { "message", entityNotFoundException.InforMessage } } });
            }
        }
    }
}
