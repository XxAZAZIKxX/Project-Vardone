using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace VardoneApi.Controllers.guilds
{
    [ApiController, Route("guilds/[controller]")]
    public class GetGuildMembersController : ControllerBase
    {
        // POST
        [HttpPost]
        public IActionResult Post()
        {
            return Task.Run(new Func<IActionResult>(() =>
            {

                return Ok();
            })).GetAwaiter().GetResult();
        }
    }
}