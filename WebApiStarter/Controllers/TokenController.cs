using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiStarter.Attributes;
using WebApiStarter.Dtos.Token;
using WebApiStarter.Services;

namespace WebApiStarter.Controllers
{
    public class TokenController : ApiControllerBase
    {
        private readonly ITokenService _tokenService;

        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        /// <summary>
        /// Generate a token by providing username and password
        /// </summary>
        [HttpPost("generate")]
        [AllowAnonymous]
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
