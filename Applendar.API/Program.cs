using System.Text.Json.Serialization;
using Applander.Infrastructure;
using Applendar.API;
using Applendar.API.Common;
using Applendar.API.Features;
using Applendar.API.Features.Events.V1;
using Applendar.API.Features.Users.V1;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
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
        // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
        // note: the specified format code will format the version as "'v'major[.minor][-status]"
        options.GroupNameFormat = "'v'VVV";
    });

;

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
        policyBuilder => policyBuilder.WithOrigins("http://localhost:7185").AllowAnyHeader().AllowAnyMethod().AllowCredentials());
});

builder.Services.AddControllers()
    .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(options => options.OperationFilter<SwaggerDefaultValues>());

string? connection = builder.Configuration.GetConnectionString("SQL_CONNECTION_STRING");

builder.Services.AddInfrastructure(configuration);
builder.Services.AddTransient<IAddEventRepository, AddEventRepository>();
builder.Services.AddTransient<IGetEventsRepository, GetEventsRepository>();
builder.Services.AddTransient<IRegisterApplendarUserRepository, RegisterApplendarUserRepository>();
builder.Services.AddTransient<IGetEventDetailsRepository, GetEventDetailsRepository>();
builder.Services.AddTransient<IGetEventsCalendarDataRepository, GetEventsCalendarDataRepository>();
builder.Services.AddTransient<IDeleteEventRepository, DeleteEventRepository>();
builder.Services.AddTransient<IUpdateEventRepository, UpdateEventRepository>();
builder.Services.AddTransient<IUpdateApplendarUserPreferencesRepository, UpdateApplendarUserPreferencesRepository>();
builder.Services.AddTransient<IGetApplendarUserDetailsRepository, GetApplendarUserDetailsRepository>();
builder.Services.AddTransient<IGetUserEventInvitationsRepository, GetUserEventInvitationsRepository>();
builder.Services.AddTransient<IUpdateUserInvitationRepository, UpdateUserInvitationRepository>();
builder.Services.AddTransient<IGetLoggedUserDataRepository, GetLoggedUserDataRepository>();

WebApplication app = builder.Build();

app.UseMiddleware<ApplendarExceptionHandlerMiddleware>();
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
IdentityModelEventSource.ShowPII = true;
app.Run();