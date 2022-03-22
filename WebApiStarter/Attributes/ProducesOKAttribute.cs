using Microsoft.AspNetCore.Mvc;

namespace WebApiStarter.Attributes
{
    public class ProducesOKAttribute : ProducesResponseTypeAttribute
    {
        public ProducesOKAttribute(Type type) : base(type, StatusCodes.Status200OK)
        {
        }
    }
}
