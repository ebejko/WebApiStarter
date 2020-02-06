using System.Threading.Tasks;
using WebApiStarter.Models;

namespace WebApiStarter.Services
{
    public interface ITokenService
    {
        public Task<TokenResponse> GenerateToken(TokenRequest request);
    }
}
