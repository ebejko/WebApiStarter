using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApiStarter.Attributes;
using WebApiStarter.Models;

namespace WebApiStarter.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TokenController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public TokenController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IOptionsSnapshot<JwtBearerOptions> jwtBearerOptions)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenValidationParameters = jwtBearerOptions.Get(JwtBearerDefaults.AuthenticationScheme).TokenValidationParameters;
        }

        /// <summary>
        /// Generate a token by providing username and password
        /// </summary>
        [HttpPost]
        [ProducesOK(typeof(TokenResponse)), ProducesNoContent, ProducesBadRequest]
        public async Task<IActionResult> Generate([FromBody]TokenRequest model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user != null)
            {
                var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

                if (result.Succeeded)
                {
                    var token = await BuildTokenForUser(user);
                    return Ok(new TokenResponse { Token = new JwtSecurityTokenHandler().WriteToken(token) });
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Refresh token
        /// </summary>
        [HttpPost]
        [Authorize]
        [ProducesOK(typeof(TokenResponse)), ProducesNoContent]
        public async Task<IActionResult> Refresh()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user != null)
            {
                var token = await BuildTokenForUser(user);
                return Ok(new TokenResponse { Token = new JwtSecurityTokenHandler().WriteToken(token) });
            }

            return NoContent();
        }

        #region Private Methods

        private async Task<JwtSecurityToken> BuildTokenForUser(IdentityUser user)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            claims.AddRange(userClaims);
            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            return new JwtSecurityToken(
                issuer: _tokenValidationParameters.ValidIssuer,
                audience: _tokenValidationParameters.ValidAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: new SigningCredentials(
                    _tokenValidationParameters.IssuerSigningKey,
                    SecurityAlgorithms.HmacSha256));
        }

        #endregion  
    }
}
