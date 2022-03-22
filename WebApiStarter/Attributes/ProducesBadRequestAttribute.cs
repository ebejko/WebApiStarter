using Microsoft.AspNetCore.Mvc;

namespace WebApiStarter.Attributes
{
    public class ProducesBadRequestAttribute : ProducesResponseTypeAttribute
    {
        public ProducesBadRequestAttribute() : base(StatusCodes.Status400BadRequest)
        {
        }
    }
}
