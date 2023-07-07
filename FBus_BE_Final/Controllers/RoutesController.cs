using FBus_BE.DTOs;
using FBus_BE.DTOs.InputDTOs;
using FBus_BE.DTOs.ListingDTOs;
using FBus_BE.DTOs.PageDTOs;
using FBus_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FBus_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoutesController : ControllerBase, IDefaultController<RoutePageRequest, RouteInputDto>
    {
        private readonly IRouteService _routeService;

        public RoutesController(IRouteService routeService)
        {
            _routeService = routeService;
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [Authorize("AdminOnly")]
        [HttpPatch("{id:int}")]
        public Task<IActionResult> ChangeStatus([FromRoute] int id, [FromBody] string status)
        {
            throw new NotImplementedException();
        }

        [NonAction]
        public Task<IActionResult> CreateWithBody([FromBody] RouteInputDto inputDto)
        {
            throw new NotImplementedException();
        }

        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RouteDto))]
        [Authorize("AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> CreateWithForm([FromForm] RouteInputDto inputDto)
        {
            int userId = Convert.ToInt32(User.FindFirst("Id").Value);
            return Ok(await _routeService.Create(userId, inputDto));
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [Authorize("AdminOnly")]
        [HttpDelete("{id:int}")]
        public Task<IActionResult> Delete([FromRoute] int id)
        {
            throw new NotImplementedException();
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RouteDto))]
        [Authorize("AdminOnly")]
        [HttpGet("{id:int}")]
        public Task<IActionResult> GetDetails([FromRoute] int id)
        {
            throw new NotImplementedException();
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DefaultPageResponse<RouteListingDto>))]
        [Authorize("AdminOnly")]
        [HttpGet]
        public Task<IActionResult> GetList([FromQuery] RoutePageRequest pageRequest)
        {
            throw new NotImplementedException();
        }

        [NonAction]
        public Task<IActionResult> UpdateWithBody([FromRoute] int id, [FromBody] RouteInputDto inputDto)
        {
            throw new NotImplementedException();
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RouteDto))]
        [Authorize("AdminOnly")]
        [HttpPut("{id:int}")]
        public Task<IActionResult> UpdateWithForm([FromRoute] int id, [FromForm] RouteInputDto inputDto)
        {
            throw new NotImplementedException();
        }
    }
}
