using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebApiStarter.Dtos.Token;

namespace WebApiStarter.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public TokenService(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IOptionsSnapshot<JwtBearerOptions> jwtBearerOptions)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _tokenValidationParameters = jwtBearerOptions?.Get(JwtBearerDefaults.AuthenticationScheme).TokenValidationParameters
                ?? throw new ArgumentNullException(nameof(jwtBearerOptions));
        }

        public async Task<TokenResponse?> GenerateToken(TokenRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user != null)
            {
                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

                if (result.Succeeded)
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

                    var expires = DateTime.UtcNow.AddHours(1);

                    var tokenHandler = new JwtSecurityTokenHandler();

                    var token = tokenHandler.CreateJwtSecurityToken(
                        issuer: _tokenValidationParameters.ValidIssuer,
                        audience: _tokenValidationParameters.ValidAudience,
                        subject: new ClaimsIdentity(claims),
                        notBefore: DateTime.UtcNow,
                        expires: expires,
                        issuedAt: DateTime.UtcNow,
                        signingCredentials: new SigningCredentials(
                            _tokenValidationParameters.IssuerSigningKey,
                            SecurityAlgorithms.HmacSha256),
                        encryptingCredentials: new EncryptingCredentials(
                            _tokenValidationParameters.TokenDecryptionKey,
                            SecurityAlgorithms.Aes256KW,
                            SecurityAlgorithms.Aes256CbcHmacSha512));

                    return new TokenResponse(tokenHandler.WriteToken(token), expires);
                }
            }

            return null;
        }
    }
}
