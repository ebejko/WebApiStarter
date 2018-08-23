using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApiStarter.Attributes
{
    public class ProducesNotFoundAttribute : ProducesResponseTypeAttribute
    {
        public ProducesNotFoundAttribute() : base(StatusCodes.Status404NotFound)
        {
        }
    }
}
