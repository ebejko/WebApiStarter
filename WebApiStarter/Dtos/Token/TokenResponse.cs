using System;

namespace WebApiStarter.Dtos.Token
{
    public class TokenResponse
    {
        public TokenResponse(string token, DateTime expires)
        {
            Token = token;
            Expires = expires;
        }

        public string Token { get; }
        public DateTime Expires { get; }
    }
}
