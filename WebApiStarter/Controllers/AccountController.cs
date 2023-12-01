using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using WebApiStarter.Attributes;
using WebApiStarter.Dtos.Account;

namespace WebApiStarter.Controllers
{
	public class AccountController : ApiControllerBase
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IUserClaimsPrincipalFactory<IdentityUser> _claimsFactory;
		private readonly BearerTokenOptions _bearerTokenOptions;
		private readonly IdentityOptions _identityOptions;

		public AccountController(
			UserManager<IdentityUser> userManager,
			IUserClaimsPrincipalFactory<IdentityUser> claimsFactory,
			IOptionsMonitor<BearerTokenOptions> bearerTokenOptionsMonitor,
			IOptions<IdentityOptions> identityOptionsAccessor)
        {
			_userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
			_claimsFactory = claimsFactory ?? throw new ArgumentNullException(nameof(claimsFactory));
			_bearerTokenOptions = bearerTokenOptionsMonitor?.Get(BearerTokenDefaults.AuthenticationScheme) ?? throw new ArgumentNullException(nameof(bearerTokenOptionsMonitor));
			_identityOptions = identityOptionsAccessor?.Value ?? throw new ArgumentNullException(nameof(identityOptionsAccessor));
		}

        /// <summary>
        /// Generate an access token by providing username and password
        /// </summary>
        [HttpPost("login")]
		[AllowAnonymous]
		[ProducesOK(typeof(AccessTokenResponse)), ProducesBadRequest]
		public async Task<IActionResult> Login([FromBody] LoginRequest model)
		{
			var user = await _userManager.FindByNameAsync(model.UserName!);

			if (user is not null && await _userManager.CheckPasswordAsync(user, model.Password!))
			{
				var principal = await _claimsFactory.CreateAsync(user);
				principal.Identities.First().AddClaim(new Claim("amr", "pwd"));

				return SignIn(principal, BearerTokenDefaults.AuthenticationScheme);
			}

			return Problem(detail: "Authentication failed", statusCode: StatusCodes.Status400BadRequest);
		}

		/// <summary>
		/// Use a refresh token to generate a new access token
		/// </summary>
		[HttpPost("refresh")]
		[AllowAnonymous]
		[ProducesOK(typeof(AccessTokenResponse)), ProducesBadRequest, ProducesUnauthrized]
		public async Task<IActionResult> Refresh([FromBody] RefreshRequest model)
		{
			var refreshTicket = _bearerTokenOptions.RefreshTokenProtector.Unprotect(model.RefreshToken);

			// Reject the /refresh attempt with a 401 if the token expired or the security stamp validation fails
			if (DateTimeOffset.UtcNow < refreshTicket?.Properties?.ExpiresUtc)
			{
				var user = await _userManager.GetUserAsync(refreshTicket.Principal);

				if (user is not null && refreshTicket.Principal.FindFirstValue(_identityOptions.ClaimsIdentity.SecurityStampClaimType) == await _userManager.GetSecurityStampAsync(user))
					return SignIn(await _claimsFactory.CreateAsync(user), authenticationScheme: BearerTokenDefaults.AuthenticationScheme);
			}

			return Challenge(BearerTokenDefaults.AuthenticationScheme);
		}

		/// <summary>
		/// Invalidate the refresh tokens by updating the security stamp
		/// </summary>
		[HttpPost("logout")]
		[ProducesOK]
		public async Task<IActionResult> Logout()
		{
			var user = await _userManager.GetUserAsync(User);
			await _userManager.UpdateSecurityStampAsync(user!);
			return SignOut(BearerTokenDefaults.AuthenticationScheme);
		}
	}
}
