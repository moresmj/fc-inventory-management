using FC.Batch.Helpers;
using FC.Batch.Helpers.DBContext;
using FC.Batch.Helpers.FileLog;
using FC.Batch.Scopes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FC.Batch.Hosts
{
    public class BackgroundHostedService<TScope>
        : IHostedService, IDisposable
        where TScope : IScopedExecuteService
    {
        public BackgroundHostedService(IServiceProvider services, IConfiguration configuration, OperateFileLogger operationLogger)
        {
            this.Services = services;
            this.Configuration = configuration;
            this.OperationLogger = operationLogger;
        }

        public IServiceProvider Services { get; }

        public IConfiguration Configuration { get; }

        protected OperateFileLogger OperationLogger { get; }

        public TaskSchedule TaskSchedule { get; } = new TaskSchedule();

        protected CancellationTokenSource cts { get; private set; }

        protected Timer executionTimer { get; private set; }

        protected OperationType OperationType { get; private set; }

        private string ScopeName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var configuration = this.Configuration.GetSection("Batch").Get<BatchConfiguration>();

            executionTimer = new Timer(PrepareProccess, null, TimeSpan.Zero, TimeSpan.FromSeconds(configuration.Repeat));

            return Task.CompletedTask;
        }

        private void PrepareProccess(dynamic state)
        {
            var executionDate = DateTime.Now;

            using (var scope = Services.CreateScope())
            {
                if (!this.TaskSchedule.IsScopeRunning)
                {
                    try
                    {
                        var scopeService = this.Initialize(scope);
                        this.ExecuteScope(scopeService, executionDate);

                    }
                    catch (HostException ex)
                    {
                        this.OperationType = OperationType.Error;
                        var logger = this.Services.GetRequiredService<ILogger<TScope>>();
                        logger.LogError(ex, ex.Message);
                    }
                }
                else
                {
                    this.OperationType = OperationType.Running;
                }
            }

            this.OperationLogger.Log(this.ScopeName, this.OperationType, executionDate, DateTime.Now);
        }

        protected virtual void ExecuteScope(TScope scopeService, DateTime start)
        {
            try
            {
                var crontab = CrontabSchedule.Parse(this.TaskSchedule.Schedule);
                var nextExecute = crontab.GetNextOccurrence(this.TaskSchedule.LastExecutedDate ?? DateTime.Now);
                nextExecute = nextExecute.AddSeconds(-nextExecute.Second);

                if (this.TaskSchedule.Active && (nextExecute < DateTime.Now ||
                    (!this.TaskSchedule.LastExecutedDate.HasValue && this.TaskSchedule.EnabledStartupRun)))
                {
                    this.OperationLogger.Log(this.ScopeName, OperationType.Started, start, DateTime.Now);
                    scopeService.Execute(this.cts.Token, null);
                    this.OperationType = OperationType.Done;
                }
            }
            catch (Exception ex)
            {
                var scopeType = scopeService.GetType();
                throw new HostException(scopeType.Name, scopeType.FullName, ex);
            }
            finally
            {
                this.TaskSchedule.IsScopeRunning = false;
                this.TaskSchedule.LastExecutedDate = DateTime.Now;
            }
        }

        private TScope Initialize(IServiceScope scope)
        {
            var scopeService = scope.ServiceProvider.GetRequiredService<TScope>();
            this.Configuration.GetSection(scopeService.ConfigSection).Bind(this.TaskSchedule);
            this.ScopeName = scopeService.GetType().Name;
            this.TaskSchedule.IsScopeRunning = true;

            return scopeService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            executionTimer?.Change(Timeout.Infinite, 0);
            this.OperationLogger.Log($"{DateTime.Now.ToString()},, Host Service {typeof(TScope).Name} has been stopped.");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            executionTimer?.Dispose();
        }
    }
}
