using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;

namespace WebApiStarter.Extensions
{
    public static class ActionContextExtensions
    {
        public static IActionResult ValidationProblemDetailsResult(this ActionContext context)
        {
            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Instance = context.HttpContext.Request.Path,
                Status = StatusCodes.Status400BadRequest,
                Type = new Uri(context.HttpContext.Request.GetEncodedUrl()).GetLeftPart(UriPartial.Authority),
                Detail = "Please refer to the errors property for additional details."
            };

            return new BadRequestObjectResult(problemDetails)
            {
                ContentTypes = { "application/problem+json" }
            };
        }
    }
}
