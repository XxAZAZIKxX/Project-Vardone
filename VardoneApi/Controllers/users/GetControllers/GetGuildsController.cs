using Microsoft.AspNetCore.Mvc;

namespace VardoneApi.Controllers.users.GetControllers
{
    [ApiController, Route("users/[controller]")]
    public class GetGuildsController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token) => BadRequest();

    }
}