using Microsoft.AspNetCore.Mvc;

namespace WebApiStarter.Attributes
{
	public class ProducesOKAttribute : ProducesResponseTypeAttribute
	{
		public ProducesOKAttribute() : base(StatusCodes.Status200OK)
		{
		}

		public ProducesOKAttribute(Type type) : base(type, StatusCodes.Status200OK)
		{
		}
	}

	public class ProducesCreatedAttribute : ProducesResponseTypeAttribute
	{
		public ProducesCreatedAttribute() : base(StatusCodes.Status201Created)
		{
		}

		public ProducesCreatedAttribute(Type type) : base(type, StatusCodes.Status201Created)
		{
		}
	}

	public class ProducesNoContentAttribute : ProducesResponseTypeAttribute
	{
		public ProducesNoContentAttribute() : base(StatusCodes.Status204NoContent)
		{
		}
	}

	public class ProducesBadRequestAttribute : ProducesResponseTypeAttribute
	{
		public ProducesBadRequestAttribute() : base(StatusCodes.Status400BadRequest)
		{
		}
	}

	public class ProducesUnauthrizedAttribute : ProducesResponseTypeAttribute
	{
		public ProducesUnauthrizedAttribute() : base(StatusCodes.Status401Unauthorized)
		{
		}
	}

	public class ProducesNotFoundAttribute : ProducesResponseTypeAttribute
	{
		public ProducesNotFoundAttribute() : base(StatusCodes.Status404NotFound)
		{
		}
	}
}
