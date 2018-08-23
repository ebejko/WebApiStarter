using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace WebApiStarter.Attributes
{
    public class ProducesOKAttribute : ProducesResponseTypeAttribute
    {
        public ProducesOKAttribute(Type type) : base(type, StatusCodes.Status200OK)
        {
        }
    }
}
