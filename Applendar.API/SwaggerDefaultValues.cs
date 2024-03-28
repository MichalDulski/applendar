using System.Text.Json;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Applendar.API;

public class SwaggerDefaultValues : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        ApiDescription? apiDescription = context.ApiDescription;

        operation.Deprecated |= apiDescription.IsDeprecated();

        foreach (ApiResponseType responseType in context.ApiDescription.SupportedResponseTypes)
        {
            string responseKey = responseType.IsDefaultResponse ? "default" : responseType.StatusCode.ToString();
            OpenApiResponse? response = operation.Responses[responseKey];

            foreach (string? contentType in response.Content.Keys)
            {
                if (!responseType.ApiResponseFormats.Any(x => x.MediaType == contentType))
                    response.Content.Remove(contentType);
            }
        }

        if (operation.Parameters == null)
            return;

        foreach (OpenApiParameter? parameter in operation.Parameters)
        {
            ApiParameterDescription description =
                apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);

            parameter.Description ??= description.ModelMetadata?.Description;

            if (parameter.Schema.Default == null && description.DefaultValue != null)
            {
                string json = JsonSerializer.Serialize(description.DefaultValue, description.ModelMetadata.ModelType);
                parameter.Schema.Default = OpenApiAnyFactory.CreateFromJson(json);
            }

            parameter.Required |= description.IsRequired;
        }
    }
}


public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;
    private readonly IConfiguration _configuration;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IConfiguration configuration)
    {
        _provider = provider;
        _configuration = configuration;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (ApiVersionDescription description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName,
                new OpenApiInfo
                {
                    Title = "Applendar API",
                    Description = "The best event organizing app API documentation",
                    Version = description.ApiVersion.ToString()
                });
        }
        options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            BearerFormat = "JWT",
            Flows = new OpenApiOAuthFlows
            {
                Implicit = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri($"{_configuration["Authentication:Auth0:Domain"]}/authorize?audience={_configuration["Authentication:Auth0:Audience"]}"),
                    TokenUrl = new Uri($"{_configuration["Authentication:Auth0:Domain"]}/oauth/token"),
                    Scopes = new Dictionary<string, string>
                    {
                        { "openid", "Open ID" },
                        // Add other scopes as needed
                    }
                }
            }
        }); 
        
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                },
                new[] { "openid" }
            }
        });
        
        
        
    }
}