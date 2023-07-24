using FBus_BE.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FBus_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IDriverService _driverService;

        public TestController(IDriverService driverService)
        {
            _driverService = driverService;
        }

        [HttpGet]
        public IActionResult Test()
        {
            return Ok("Ok");
        }
    }
}
