using FBus_BE.DTOs.InputDTOs;
using FBus_BE.DTOs.PageDTOs;
using FBus_BE.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FBus_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase, IDefaultController<DriverPageRequest, DriverInputDto>
    {
        public TestController() { }

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
        [NonAction]
        public Task<IActionResult> GetList([FromQuery] DriverPageRequest pageRequest)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public IActionResult Test()
        {
            return Ok("Ok");
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
