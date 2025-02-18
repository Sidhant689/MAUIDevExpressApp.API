using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MAUIDevExpressApp.API.Filters
{
    public class SwaggerJwtAuthenticationFilter :IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Security == null)
                operation.Security = new List<OpenApiSecurityRequirement>();

            operation.Security.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
        });

            // Modify parameters to automatically append "Bearer " prefix
            var authParameter = operation.Parameters?.FirstOrDefault(p => p.Name == "Authorization");
            if (authParameter != null)
            {
                authParameter.Description = "Enter only the JWT token. The 'Bearer ' prefix is added automatically.";
            }
        }
    }
}