using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TodoApi.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;

namespace TodoApi
{
    public class Startup
    {
        public static IConfigurationRoot Configuration { get; set; }

        static Startup()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {

            string connection = Configuration.GetConnectionString("TodoContext");
            services.AddDbContext<TodoContext>(opt => opt.UseSqlServer(connection));
            services.AddCors(options => {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseCors("CorsPolicy");
            app.UseMvc();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder()
              .ConfigureAppConfiguration((ctx, cfg) =>
              {
                  cfg.SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json", true) // require the json file!
          .AddEnvironmentVariables();
              })
              .ConfigureLogging((ctx, logging) => { }) // No logging
              .UseStartup<Startup>()
              .UseSetting("DesignTime", "true")
              .Build();
        }
    }
}
