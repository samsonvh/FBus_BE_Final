using FBus_BE.DTOs;
using FBus_BE.DTOs.PageDTOs;
using FBus_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FBus_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase, IDefaultController<AccountPageRequest, AccountDto>
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DefaultPageResponse<AccountDto>))]
        [Authorize("AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] AccountPageRequest pageRequest)
        {
            return Ok(await _accountService.GetList(pageRequest));
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountDto))]
        [Authorize("AdminOnly")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDetails([FromRoute] int id)
        {
            return Ok(await _accountService.GetDetails(id));
        }

        [NonAction]
        public Task<IActionResult> CreateWithForm([FromForm] AccountDto inputDto)
        {
            throw new NotImplementedException();
        }

        [NonAction]
        public Task<IActionResult> CreateWithBody([FromBody] AccountDto inputDto)
        {
            throw new NotImplementedException();
        }

        [NonAction]
        public Task<IActionResult> UpdateWithForm([FromRoute] int id, [FromForm] AccountDto inputDto)
        {
            throw new NotImplementedException();
        }

        [NonAction]
        public Task<IActionResult> UpdateWithBody([FromRoute] int id, [FromBody] AccountDto inputDto)
        {
            throw new NotImplementedException();
        }

        [NonAction]
        public Task<IActionResult> ChangeStatus([FromRoute] int id, [FromBody] string status)
        {
            throw new NotImplementedException();
        }

        [NonAction]
        public Task<IActionResult> Delete([FromRoute] int id)
        {
            throw new NotImplementedException();
        }
    }
}
