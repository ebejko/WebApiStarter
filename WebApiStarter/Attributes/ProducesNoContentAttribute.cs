using Microsoft.AspNetCore.Mvc;

namespace WebApiStarter.Attributes
{
    public class ProducesNoContentAttribute : ProducesResponseTypeAttribute
    {
        public ProducesNoContentAttribute() : base(StatusCodes.Status204NoContent)
        {
        }
    }
}
