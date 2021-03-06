using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TravellerSpot.Configuration.Neo4jDatabaseSettings;
using TravellerSpot.Contexts;
using TravellerSpot.Services;
using TravellerSpot.QueryHelper;

namespace TravellerSpot
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<Neo4jDatabaseSettings>(Configuration.GetSection(nameof(Neo4jDatabaseSettings)));
            services.AddSingleton<INeo4jDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<Neo4jDatabaseSettings>>().Value);
            services.AddControllers();
            services.AddSingleton<DatabaseContext>();
            services.AddSingleton<PersonService>();
            services.AddSingleton<TripService>();
            services.AddSingleton<SpotService>();
            services.AddSingleton<RedisService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,RedisService redisService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            redisService.Connect();
        }
    }
}
