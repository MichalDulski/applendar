using Applendar.API.Features.Events.V1;
using Applendar.API.Features.Users.V1;

namespace Applendar.API.Features;

public static class DependencyInjection
{
    public static IServiceCollection AddFeaturesDependencies(this IServiceCollection services)
    {
        services.AddTransient<IAddEventRepository, AddEventRepository>();
        services.AddTransient<IGetEventsRepository, GetEventsRepository>();
        services.AddTransient<IRegisterApplendarUserRepository, RegisterApplendarUserRepository>();
        services.AddTransient<IGetEventDetailsRepository, GetEventDetailsRepository>();
        services.AddTransient<IGetEventsCalendarDataRepository, GetEventsCalendarDataRepository>();
        services.AddTransient<IDeleteEventRepository, DeleteEventRepository>();
        services.AddTransient<IUpdateEventRepository, UpdateEventRepository>();

        services.AddTransient<IUpdateApplendarUserPreferencesRepository,
            UpdateApplendarUserPreferencesRepository>();

        services.AddTransient<IGetApplendarUserProfileRepository, GetApplendarUserProfileRepository>();
        services.AddTransient<IGetUserEventInvitationsRepository, GetUserEventInvitationsRepository>();
        services.AddTransient<IUpdateUserInvitationRepository, UpdateUserInvitationRepository>();
        services.AddTransient<IGetLoggedUserDataRepository, GetLoggedUserDataRepository>();
        services.AddTransient<LogUserLastActivityMiddleware>();

        return services;
    }
}