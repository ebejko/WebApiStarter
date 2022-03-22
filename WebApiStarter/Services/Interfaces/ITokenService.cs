using WebApiStarter.Dtos.Token;

namespace WebApiStarter.Services
{
    public interface ITokenService
    {
        public Task<TokenResponse?> GenerateToken(TokenRequest request);
    }
}
