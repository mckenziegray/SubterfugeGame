using Microsoft.Extensions.DependencyInjection;

namespace Subterfuge
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<GameService>();
        }
    }
}
