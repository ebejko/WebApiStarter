using System.ComponentModel.DataAnnotations;

namespace WebApiStarter.Dtos.Token
{
    public class TokenRequest
    {
        /// <example>admin</example>
        [Required]
        public string? UserName { get; set; }

        /// <example>Pa$$w0rd</example>
        [Required]
        public string? Password { get; set; }
    }
}
