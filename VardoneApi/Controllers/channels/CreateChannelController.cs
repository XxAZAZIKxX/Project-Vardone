using Microsoft.AspNetCore.Mvc;

namespace VardoneApi.Controllers.channels
{
    [ApiController, Route("channels/[controller]")]
    public class CreateChannelController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] string username, [FromHeader] string token) => BadRequest();
    }
}