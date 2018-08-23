using System.ComponentModel.DataAnnotations;

namespace WebApiStarter.Models
{
    public class TokenRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
