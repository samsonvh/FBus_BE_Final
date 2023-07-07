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
    public class BusesController : ControllerBase, IDefaultController<BusPageRequest, BusInputDto>
    {
        private readonly IBusService _busService;

        public BusesController(IBusService busService)
        {
            _busService = busService;
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [Authorize("AdminOnly")]
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> ChangeStatus([FromRoute] int id, [FromBody] string status)
        {
            try
            {
                return Ok(await _busService.ChangeStatus(id, status));
            } catch (EntityNotFoundException entityNotFoundException)
            {
                return BadRequest(new ErrorDto { Title = "Entity Not Found", Errors = entityNotFoundException.InforMessage});
            }
        }

        [NonAction]
        public Task<IActionResult> CreateWithBody([FromBody] BusInputDto inputDto)
        {
            throw new NotImplementedException();
        }

        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BusDto))]
        [Authorize("AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> CreateWithForm([FromForm] BusInputDto inputDto)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst("Id").Value);
                return Ok(await _busService.Create(userId, inputDto));
            }
            catch (DuplicateException duplicateException)
            {
                return BadRequest(new ErrorDto { Title = "Duplicated", Errors = duplicateException.GetErrors() });
            }
            catch (EntityNotFoundException entityNotFoundException)
            {
                return BadRequest(new ErrorDto { Title = "Entity Not Found", Errors = entityNotFoundException.InforMessage });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [Authorize("AdminOnly")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                return Ok(await _busService.Delete(id));
            }
            catch (EntityNotFoundException entityNotFoundException)
            {
                return BadRequest(new ErrorDto { Title = "Entity Not Found", Errors = entityNotFoundException.InforMessage });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BusDto))]
        [Authorize("AdminOnly")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDetails([FromRoute] int id)
        {
            try
            {
                return Ok(await _busService.GetDetails(id));
            } catch (EntityNotFoundException entityNotFoundException)
            {
                return BadRequest(new ErrorDto { Title = "Entity Not Found", Errors = entityNotFoundException.InforMessage });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DefaultPageResponse<BusListingDto>))]
        [Authorize("AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] BusPageRequest pageRequest)
        {
            return Ok(await _busService.GetList(pageRequest));
        }

        [NonAction]
        public Task<IActionResult> UpdateWithBody([FromRoute] int id, [FromBody] BusInputDto inputDto)
        {
            throw new NotImplementedException();
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BusDto))]
        [Authorize("AdminOnly")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateWithForm([FromRoute] int id, [FromForm] BusInputDto inputDto)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst("Id").Value);
                return Ok(await _busService.Update(userId, inputDto, id));
            }
            catch (DuplicateException duplicateException)
            {
                return BadRequest(new ErrorDto { Title = "Duplicated", Errors = duplicateException.GetErrors() });
            }
            catch (EntityNotFoundException entityNotFoundException)
            {
                return BadRequest(new ErrorDto { Title = "Entity Not Found", Errors = entityNotFoundException.InforMessage });
            }
        }
    }
}
