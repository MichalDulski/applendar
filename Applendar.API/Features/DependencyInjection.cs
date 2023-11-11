using Applendar.API.Features.Events.V1.AddEvent;
using Applendar.API.Features.Events.V1.ChangeEventLocation;
using Applendar.API.Features.Events.V1.DeleteEvent;
using Applendar.API.Features.Events.V1.GetEventDetails;
using Applendar.API.Features.Events.V1.GetEvents;
using Applendar.API.Features.Events.V1.GetEventsCalendarData;
using Applendar.API.Features.Events.V1.UpdateEvent;
using Applendar.API.Features.Users.V1.GetApplendarUserDetails;
using Applendar.API.Features.Users.V1.GetLoggedUserData;
using Applendar.API.Features.Users.V1.GetUserEventInvitations;
using Applendar.API.Features.Users.V1.RegisterApplendarUser;
using Applendar.API.Features.Users.V1.UpdateApplendarUserPreferences;
using Applendar.API.Features.Users.V1.UpdateUserInvitation;

namespace Applendar.API.Features;

public static class DependencyInjection
{
    public static IServiceCollection AddFeaturesDependencies(this IServiceCollection services)
    {
        services.AddTransient<IAddEventRepository, AddEventRepository>();
        services.AddTransient<IGetEventsRepository, GetEventsRepository>();
        services.AddTransient<IGetEventDetailsRepository, GetEventDetailsRepository>();
        services.AddTransient<IGetEventsCalendarDataRepository, GetEventsCalendarDataRepository>();
        services.AddTransient<IDeleteEventRepository, DeleteEventRepository>();
        services.AddTransient<IUpdateEventRepository, UpdateEventRepository>();
        services.AddTransient<IChangeEventLocationRepository, ChangeEventLocationRepository>();
        
        services.AddTransient<IUpdateApplendarUserPreferencesRepository,
            UpdateApplendarUserPreferencesRepository>();

        services.AddTransient<IRegisterApplendarUserRepository, RegisterApplendarUserRepository>();
        services.AddTransient<IGetApplendarUserProfileRepository, GetApplendarUserProfileRepository>();
        services.AddTransient<IGetUserEventInvitationsRepository, GetUserEventInvitationsRepository>();
        services.AddTransient<IUpdateUserInvitationRepository, UpdateUserInvitationRepository>();
        services.AddTransient<IGetLoggedUserDataRepository, GetLoggedUserDataRepository>();
        services.AddTransient<LogUserLastActivityMiddleware>();

        return services;
    }
}