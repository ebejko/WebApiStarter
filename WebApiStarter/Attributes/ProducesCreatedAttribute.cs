using Microsoft.AspNetCore.Mvc;

namespace WebApiStarter.Attributes
{
    public class ProducesCreatedAttribute : ProducesResponseTypeAttribute
    {
        public ProducesCreatedAttribute() : base(StatusCodes.Status201Created)
        {
        }

        public ProducesCreatedAttribute(Type type) : base(type, StatusCodes.Status201Created)
        {
        }
    }
}
