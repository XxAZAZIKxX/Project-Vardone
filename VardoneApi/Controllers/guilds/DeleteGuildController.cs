using Microsoft.AspNetCore.Mvc;

namespace VardoneApi.Controllers.guilds
{
    [ApiController, Route("guilds/[controller]")]
    public class DeleteGuildController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] string username, [FromHeader] string token) => BadRequest();
    }
}