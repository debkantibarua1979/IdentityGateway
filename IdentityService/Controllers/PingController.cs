namespace IdentityService.Controllers;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("ping")]
public class PingController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("pong");
}