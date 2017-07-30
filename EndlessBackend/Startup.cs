using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EndlessBackend.Services;
using EndlessConfiguration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EndlessBackend
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("/home/bo20202/endless-bot/sf_EndlessBot/EndlessBackend/appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            Config.LoadConfig("/home/bo20202/bot/config.json");
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddTransient<ServerStatusService>();
            services.AddSingleton<GameServerService>();
            services.AddSingleton<UpdateService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            
            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "default", template: "{controller}/{action}");
            });
        }
    }
}
