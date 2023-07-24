using FBus_BE.DTOs.InputDTOs;
using FBus_BE.DTOs.PageDTOs;
using FBus_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FBus_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripStatusesController : ControllerBase, IDefaultController<TripStatusPageRequest, TripStatusInputDto>
    {
        private readonly ITripStatusService _tripStatusService;

        public TripStatusesController(ITripStatusService tripStatusService) {
            _tripStatusService = tripStatusService;
        }
        
        [NonAction]
        public Task<IActionResult> ChangeStatus([FromRoute] int id, [FromBody] string status)
        {
            throw new NotImplementedException();
        }

        [NonAction]
        public Task<IActionResult> CreateWithBody([FromBody] TripStatusInputDto inputDto)
        {
            throw new NotImplementedException();
        }

        [Authorize("DriverOnly")]
        [HttpPost]
        public async Task<IActionResult> CreateWithForm([FromForm] TripStatusInputDto inputDto)
        {
            int userId = Convert.ToInt32(User.FindFirst("Id"));
            return Ok(await _tripStatusService.Create(userId, inputDto));
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

        [Authorize]
        [HttpGet]
        public Task<IActionResult> GetList([FromQuery] TripStatusPageRequest pageRequest)
        {
            throw new NotImplementedException();
        }

        [NonAction]
        public Task<IActionResult> UpdateWithBody([FromRoute] int id, [FromBody] TripStatusInputDto inputDto)
        {
            throw new NotImplementedException();
        }

        [NonAction]
        public Task<IActionResult> UpdateWithForm([FromRoute] int id, [FromForm] TripStatusInputDto inputDto)
        {
            throw new NotImplementedException();
        }
    }
}
