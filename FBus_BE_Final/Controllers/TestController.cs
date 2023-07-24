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

        [NonAction]
        public Task<IActionResult> ChangeStatus([FromRoute] int id, [FromBody] string status)
        {
            throw new NotImplementedException();
        }

        [NonAction]
        public Task<IActionResult> CreateWithBody([FromBody] DriverInputDto inputDto)
        {
            throw new NotImplementedException();
        }

        [NonAction]
        public Task<IActionResult> CreateWithForm([FromForm] DriverInputDto inputDto)
        {
            throw new NotImplementedException();
        }

        [NonAction]
        public Task<IActionResult> Delete([FromRoute] int id)
        {
            throw new NotImplementedException();
        }

        [NonAction]
        public Task<IActionResult> GetDetails([FromRoute] int id)
        {
            throw new NotImplementedException();
        }
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] DriverPageRequest pageRequest)
        {
            return Ok("OK");
        }

        [NonAction]
        public Task<IActionResult> UpdateWithBody([FromRoute] int id, [FromBody] DriverInputDto inputDto)
        {
            throw new NotImplementedException();
        }

        [NonAction]
        public Task<IActionResult> UpdateWithForm([FromRoute] int id, [FromForm] DriverInputDto inputDto)
        {
            throw new NotImplementedException();
        }
    }
}
