using System.ComponentModel.DataAnnotations;

namespace WebApiStarter.Dtos.Account
{
	public class RefreshRequest
	{
		[Required]
		public string? RefreshToken { get; set; }
	}
}
