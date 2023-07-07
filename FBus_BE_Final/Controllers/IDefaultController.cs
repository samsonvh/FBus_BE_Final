using FBus_BE.DTOs.PageDTOs;
using Microsoft.AspNetCore.Mvc;

namespace FBus_BE.Controllers
{
    public interface IDefaultController<P,I>
    {
        Task<IActionResult> GetList([FromQuery] P pageRequest);
        Task<IActionResult> GetDetails([FromRoute] int id);
        Task<IActionResult> CreateWithForm([FromForm] I inputDto);
        Task<IActionResult> CreateWithBody([FromBody] I inputDto);
        Task<IActionResult> UpdateWithForm([FromRoute] int id, [FromForm] I inputDto);
        Task<IActionResult> UpdateWithBody([FromRoute] int id, [FromBody] I inputDto);
        Task<IActionResult> ChangeStatus([FromRoute] int id, [FromBody] string status);
        Task<IActionResult> Delete([FromRoute] int id);
    }
}
