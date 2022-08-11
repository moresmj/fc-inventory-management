using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using InventorySystemAPI.Context;
using Microsoft.EntityFrameworkCore;
using InventorySystemAPI.Models;
using FluentValidation.AspNetCore;

namespace InventorySystemAPI
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

            services.AddDbContextPool<FloorCenterContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddMvc(opt =>
            {
                opt.Filters.Add(typeof(ValidateModelStateFilter));
            }).AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>());

            //for CORS to allow connection between API and angular
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowAnyOrigin();
            }));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors("MyPolicy");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                        name: "routing1",
                        template: "api/{controller}/{action}"
                    );
                routes.MapRoute(
                        name: "routing2",
                        template: "api/{controller}/{action}/{id?}"
                    );
            });

        }
    }
}
