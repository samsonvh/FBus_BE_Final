using FBus_BE.DTOs;
using FBus_BE.DTOs.InputDTOs;
using FBus_BE.DTOs.ListingDTOs;
using FBus_BE.DTOs.PageDTOs;
using FBus_BE.Exceptions;
using FBus_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FBus_BE.Controllers
{
    [Route("api/Drivers")]
    [ApiController]
    public class DriversController2 : ControllerBase, IDefaultController<DriverPageRequest, DriverInputDto>
    {
        private readonly IDriverService _driverService;

        public DriversController2(IDriverService driverService)
        {
            _driverService = driverService;
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [Authorize("AdminOnly")]
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> ChangeStatus([FromRoute] int id, [FromBody] string status)
        {
            try
            {
                return Ok(await _driverService.ChangeStatus(id, status));
            }
            catch (EntityNotFoundException entityNotFoundException)
            {
                return BadRequest(new ErrorDto { Title = "Entity Not Found", Errors = { { "message", entityNotFoundException.InforMessage } } });
            }
        }

        [NonAction]
        public Task<IActionResult> CreateWithBody([FromBody] DriverInputDto inputDto)
        {
            throw new NotImplementedException();
        }

        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(DriverDto))]
        [Authorize("AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> CreateWithForm([FromForm] DriverInputDto inputDto)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst("Id").Value);
                DriverDto? driver = await _driverService.Create(userId, inputDto);
                return Ok(driver);
            }
            catch (DuplicateException duplicateException)
            {
                return BadRequest(new ErrorDto { Title = "Duplicated", Errors = duplicateException.GetErrors() });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [Authorize("AdminOnly")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                return Ok(await _driverService.Delete(id));
            }
            catch (EntityNotFoundException entityNotFoundException)
            {
                return BadRequest(new ErrorDto { Title = "Entity Not Found", Errors = new Dictionary<string, string>() { { "message", entityNotFoundException.InforMessage } } });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DriverDto))]
        [Authorize("AdminOnly")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDetails([FromRoute] int id)
        {
            return Ok(await _driverService.GetDetails(id));
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DefaultPageResponse<DriverListingDto>))]
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] DriverPageRequest pageRequest)
        {
            //return Ok(await _driverService.GetList(pageRequest));
            return Ok("Ok");
        }

        [NonAction]
        public Task<IActionResult> UpdateWithBody([FromRoute] int id, [FromBody] DriverInputDto inputDto)
        {
            throw new NotImplementedException();
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DriverDto))]
        [Authorize("AdminOnly")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateWithForm([FromRoute] int id, [FromForm] DriverInputDto inputDto)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst("Id").Value);
                DriverDto? driver = await _driverService.Update(userId, inputDto, id);
                return Ok(driver);
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
