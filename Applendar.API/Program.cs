using System.Text.Json.Serialization;
using Applendar.Infrastructure;
using Applendar.API;
using Applendar.API.Common;
using Applendar.API.Features;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Host.UseSerilog(((context, services,
    options) => options.ReadFrom.Configuration(configuration)));

builder.Services.AddProblemDetails();

builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.ReportApiVersions = true;
        options.AssumeDefaultVersionWhenUnspecified = true;
    })
    .AddMvc()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = $"{configuration["Authentication:Auth0:Domain"]}/";
    options.Audience = configuration["Authentication:Auth0:DomainAudience"];
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidIssuer = configuration["Authentication:Auth0:Domain"],
        ValidateAudience = true,
        ValidAudience = configuration["Authentication:Auth0:Audience"],
        ValidateLifetime = true,
    };
    options.Events = new JwtBearerEvents()
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine(context.Exception);

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policyBuilder => policyBuilder
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

builder.Services.AddControllers()
    .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(options => options.OperationFilter<SwaggerDefaultValues>());

builder.Services.AddInfrastructure(configuration);
builder.Services.AddFeaturesDependencies();

WebApplication app = builder.Build();

Console.WriteLine(configuration["Migrate:MigrateOnStartup"]);
var isLoadMigrateSuccess = bool.TryParse(configuration["Migrate:MigrateOnStartup"], out var migrateOnStartup);
if (isLoadMigrateSuccess && migrateOnStartup)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplendarDbContext>();
    db.Database.Migrate();
}

app.UseMiddleware<ApplendarExceptionHandlerMiddleware>();
app.UseMiddleware<LogUserLastActivityMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    foreach (ApiVersionDescription description in app.DescribeApiVersions())
    {
        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName);
    }

    options.OAuthAppName("ApplendarAPI");
    options.OAuthClientId(configuration["Authentication:Auth0:ClientId"]);
    options.OAuthClientSecret(configuration["Authentication:Auth0:ClientSecret"]);
    options.OAuthUsePkce();
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowSpecificOrigin");
app.MapControllers();
app.UseStaticFiles();

app.Run();