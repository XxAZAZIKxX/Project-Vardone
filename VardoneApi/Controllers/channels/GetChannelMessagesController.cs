using Microsoft.AspNetCore.Mvc;

namespace VardoneApi.Controllers.channels
{
    [ApiController, Route("channels/[controller]")]
    public class GetChannelMessagesController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] string username, [FromHeader] string token) => BadRequest();
    }
}