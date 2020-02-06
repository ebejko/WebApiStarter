using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebApiStarter.Attributes;
using WebApiStarter.Models;
using WebApiStarter.Services;

namespace WebApiStarter.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        /// <summary>
        /// Generate a token by providing username and password
        /// </summary>
        [HttpPost]
        [ProducesOK(typeof(TokenResponse)), ProducesNoContent, ProducesBadRequest]
        public async Task<IActionResult> Generate([FromBody]TokenRequest model)
        {
            var token = await _tokenService.GenerateToken(model);

            if (token != null)
                return Ok(token);
            else
                return NoContent();
        }
    }
}
