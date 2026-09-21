    using Hangfire.Dashboard;

namespace EmailPlatform.Infrastructure.BackgroundJobs;

public class AllowAllHangfireAuthorizationFilter
    : IDashboardAuthorizationFilter
{
    public bool Authorize(
        DashboardContext context)
    {
        return true;
    }
}