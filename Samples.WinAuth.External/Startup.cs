using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Samples.WinAuth.Shared;
using Microsoft.OpenApi.Models;
using Samples.WinAuth.External.Controllers;

namespace Samples.WinAuth.External
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
            services.AddControllers();
            services.AddMockClaimsPrincipal(Configuration, "MockClaimsPrincipal");

            services.Configure<IISOptions>(opts =>
            {
                opts.AutomaticAuthentication = true;
            });

            var internalApiUrl = Configuration["Apis:Internal"];
            services.AddHttpClient<InternalApiClient>(config =>
            {
                config.BaseAddress = new System.Uri(internalApiUrl);
            });
            services.AddTransient<InternalApiClient>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "External API", Version = "v1" });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseMockClaimsPrincipal();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "External API");
            });

        }
    }
}
