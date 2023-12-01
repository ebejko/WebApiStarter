using System.ComponentModel.DataAnnotations;

namespace WebApiStarter.Dtos.Account
{
	public class LoginRequest
	{
		/// <example>admin</example>
		[Required]
		public string? UserName { get; set; }

		/// <example>Pa$$w0rd</example>
		[Required]
		public string? Password { get; set; }
	}
}
