using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using FC.Batch.Scopes;
using AutoMapper;
using FC.Batch.Helpers;
using FC.Batch.Helpers.DBContext;
using Microsoft.EntityFrameworkCore;
using FC.Batch.Scopes.Sample;
using FC.Batch.Scopes.WHInventoryUpdater;
using FC.Batch.Scopes.STInventoryUpdater;
using FC.Batch.Helpers.FileLog;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace FC.Batch
{
    public class Startup
        : DebugStartup
    {
        public Startup(IConfiguration configuration, OperateFileLogger operateFileLogger)
            : base(configuration)
        {
            this.OperationLogger = operateFileLogger;
        }

        protected OperateFileLogger OperationLogger { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<FC.Api.Helpers.DataContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddAutoMapper();

            services.AddMvc();

            // configure strongly typed settings objects
            var batchSection = Configuration.GetSection("Batch");
            services.Configure<BatchConfiguration>(batchSection);

            //Hosts
            services.AddSingleton<IHostedService, Hosts.BackgroundHostedService<ISTInventoryUpdaterService>>();

            //Scopes
            services.AddScoped<ISTInventoryUpdaterService, STInventoryUpdaterService>();


            //Hosts
            services.AddSingleton<IHostedService, Hosts.BackgroundHostedService<IWHInventoryUpdaterService>>();

            //Scopes
            services.AddScoped<IWHInventoryUpdaterService, WHInventoryUpdaterService>();

            //
            base.ConfigureServices(services);

            this.OperationLogger.Log($"{DateTime.Now.ToString()},, Startup.ConfigureServices triggered.");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //var viewError = app.ApplicationServices.GetRequiredService<ViewError>();
            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Batch is running...." + Environment.NewLine + Environment.NewLine + viewError.GetStringError());
            //});

            app.UseMvcWithDefaultRoute();
        }
    }
}
