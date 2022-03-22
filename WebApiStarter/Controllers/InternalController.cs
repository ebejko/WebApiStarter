using Microsoft.AspNetCore.Mvc;

namespace WebApiStarter.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/[controller]/[action]")]
    public class InternalController : ControllerBase
    {
        [HttpGet, HttpPost, HttpPut, HttpPatch, HttpDelete]
        public IActionResult Default()
        {
            return NotFound();
        }

        [Route("exception")]
        [HttpGet, HttpPost, HttpPut, HttpPatch, HttpDelete]
        public IActionResult Exception() 
        {
            if (Response.StatusCode == StatusCodes.Status200OK)
            {
                return NotFound();
            }

            return Problem();
        }
    }
}
