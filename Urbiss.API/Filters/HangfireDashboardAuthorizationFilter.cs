using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace Urbiss.API.Filters
{
    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return context.GetHttpContext().User.IsInRole("Admin");
        }
    }
}
