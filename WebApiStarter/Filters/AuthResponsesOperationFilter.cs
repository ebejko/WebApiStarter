using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApiStarter.Constants;

namespace WebApiStarter.Filters
{
    public class AuthResponsesOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var attributes = context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
                                .Union(context.MethodInfo.GetCustomAttributes(true)) 
                                ?? Enumerable.Empty<object>();

            if (attributes.OfType<IAllowAnonymous>().Any())
            {
                return;
            }

            var authAttributes = attributes.OfType<IAuthorizeData>();

            if (authAttributes.Any())
            {
                operation.Responses["401"] = new OpenApiResponse { Description = "Unauthorized" };

                if (authAttributes.Any(att => !String.IsNullOrWhiteSpace(att.Roles) || !String.IsNullOrWhiteSpace(att.Policy)))
                {
                    operation.Responses["403"] = new OpenApiResponse { Description = "Forbidden" };
                }

                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme 
                            {
                                Reference = new OpenApiReference 
                                { 
                                    Id = AuthenticationConstants.SchemeName, 
                                    Type = ReferenceType.SecurityScheme 
                                }
                            },
                            Array.Empty<string>()
                        }
                    }
                };
            }
        }
    }
}
