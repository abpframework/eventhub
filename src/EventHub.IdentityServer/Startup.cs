using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace EventHub
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication<EventHubIdentityServerModule>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.InitializeApplication();
        }
    }
}
