using Microsoft.AspNetCore.Mvc;

namespace VardoneApi.Controllers.guilds
{
    [ApiController, Route("guilds/[controller]")]
    public class CreateController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post() => BadRequest();
    }
}