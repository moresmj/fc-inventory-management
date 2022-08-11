using FC.Batch.Scopes;
using FC.Batch.Scopes.Sample;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Batch.Helpers
{
    public abstract class DebugStartup
    {
        public DebugStartup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            var configuration = this.Configuration.GetSection("Batch").Get<BatchConfiguration>();

            if (configuration.RunTest)
            {
                this.RegisterTestBatch(services);
            }

            if (configuration.TargetDebug != null)
            {
                var scopes = services.Where(service =>
                   typeof(IScopedExecuteService).IsAssignableFrom(service.ServiceType)).ToList();

                var list = scopes.FindAll(_scope => !configuration.TargetDebug.Contains(_scope.ImplementationType.Name));

                if (list.Count > 0)
                {
                    var hosts = services.Where(service => service.ServiceType == typeof(IHostedService)).ToList();

                    foreach (var scope in list)
                    {
                        var removeHost = hosts.FindAll(host => host.ImplementationType.IsGenericType && host.ImplementationType.GetGenericArguments()[0] == scope.ServiceType);
                        foreach (var host in removeHost)
                        {
                            services.Remove(host);
                        }
                    }
                }
            }
        }

        protected virtual void RegisterTestBatch(IServiceCollection services)
        {
            #region Test Batch
            //Hosts
            services.AddSingleton<IHostedService, Hosts.BackgroundHostedService<IScopedTestErrorService>>();
            services.AddSingleton<IHostedService, Hosts.BackgroundHostedService<IScopedTestFinishedService>>();

            //Scopes
            services.AddScoped<Scopes.Sample.IScopedTestErrorService, Scopes.Sample.ScopedTestErrorService>();
            services.AddScoped<Scopes.Sample.IScopedTestFinishedService, Scopes.Sample.ScopedTestFinishedService>();
            #endregion
        }
    }
}
