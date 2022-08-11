using System.Text;
using AutoMapper;
using FC.Api.Helpers;
using FC.Api.Helpers.Notification;
using FC.Api.Services.Companies;
using FC.Api.Services.Dashboard;
using FC.Api.Services.Items;
using FC.Api.Services.Logger;
using FC.Api.Services.NotificationHub;
using FC.Api.Services.Sizes;
using FC.Api.Services.Stores;
using FC.Api.Services.Stores.AdvanceOrder;
using FC.Api.Services.Stores.Deliveries;
using FC.Api.Services.Stores.Releasing;
using FC.Api.Services.Stores.Returns;
using FC.Api.Services.Stores.StockHistory;
using FC.Api.Services.Users;
using FC.Api.Services.Warehouses;
using FC.Api.Services.Warehouses.ModifyTonality;
using FC.Api.Services.Warehouses.Receive_Items;
using FC.Api.Services.Warehouses.Returns;
using FC.Api.Services.Warehouses.StockHistory;
using FC.Api.Validators;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace FC.Api
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
            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithOrigins("http://localhost:4200");
            }));

            services.AddSignalR();



            services.AddDbContextPool<DataContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddMvc(opt =>
            {
                opt.Filters.Add(typeof(ValidateModelStateFilter));
            }).AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>())
            .AddJsonOptions(
            options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            services.AddAutoMapper();

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // configure DI for application services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IWarehouseService, WarehouseService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<ICategoryParentService, CategoryParentService>();
            services.AddScoped<ICategoryChildService, CategoryChildService>();
            services.AddScoped<ICategoryGrandChildService, CategoryGrandChildService>();
            services.AddScoped<ISizeService, SizeService>();
            services.AddScoped<IWHReceiveService, WHReceiveService>();
            services.AddScoped<IWHStockService, WHStockService>();
            services.AddScoped<ISTOrderService, STOrderService>();
            services.AddScoped<ISTDeliveryService, STDeliveryService>();
            services.AddScoped<ISTStockService, STStockService>();
            services.AddScoped<ISTSalesService, STSalesService>();
            services.AddScoped<ISTTransferService, STTransferService>();
            

            #region Releasing
            services.AddScoped<IForClientOrderService, ForClientOrderService>();
            services.AddScoped<ISameDaySalesService, SameDaySalesService>();
            services.AddScoped<ISalesOrderService, SalesOrderService>();
            services.AddScoped<ITransferService, TransferService>();
            services.AddScoped<Services.Stores.Releasing.IPurchaseReturnService, Services.Stores.Releasing.PurchaseReturnService>();
            #endregion

            #region Returns
            services.AddScoped<Services.Stores.Returns.IPurchaseReturnService, Services.Stores.Returns.PurchaseReturnService>();
            services.AddScoped<IReturnService, ReturnService>();
            services.AddScoped<IClientReturnService, ClientReturnService>();
            #endregion

            services.AddScoped<IWHDeliveryService, WHDeliveryService>();

            services.AddScoped<IReturnsForDeliveriesService, ReturnsForDeliveriesService>();

            services.AddScoped<IBranchOrdersService, BranchOrdersService>();

            services.AddScoped<IMainDashboardService, MainDashboardService>();
            services.AddScoped<IStoreDashboardService, StoreDashboardService>();
            services.AddScoped<IWarehouseDashboardService, WarehouseDashboardService>();
            services.AddScoped<ILogisticsDashboardService, LogisticsDashboardService>();

            services.AddScoped<IReturnsForReceivingService, ReturnsForReceivingService>();

            services.AddScoped<Services.Stores.ReceiveItems.IClientReturnService, Services.Stores.ReceiveItems.ClientReturnService>();

            services.AddScoped<IStoreReturnsForDeliveryService, StoreReturnsForDeliveryService>();

            services.AddScoped<IModifyTonalityService, ModifyTonalityService>();

            #region Stock History

            //  Warehouse
            services.AddScoped<IWarehouseStockHistoryService, WarehouseStockHistoryService>();

            //  Store
            services.AddScoped<IStoreStockHistoryService, StoreStockHistoryService>();

            #endregion

            #region Physical Count

            services.AddScoped<Services.Warehouses.PhysicalCount.IUploadPhysicalCountService, Services.Warehouses.PhysicalCount.UploadPhysicalCountService>();
            services.AddScoped<Services.Stores.PhysicalCount.IUploadPhysicalCountService, Services.Stores.PhysicalCount.UploadPhysicalCountService>();

            #endregion

            #region User trail
            services.AddScoped<Services.UserTrails.IUserTrailService, Services.UserTrails.UserTrailService>();
            services.AddScoped<UserTrailActionFilter>();
            services.AddScoped<NotifyWarehouseFilterAttribute>();
            services.AddScoped<NotifyActionFilter>();
            #endregion

            #region User trail
            services.AddScoped<ILoggerService, LoggerService>();
            #endregion

            #region advance order
            services.AddScoped<ISTAdvanceOrderService, STAdvanceOrderService>();
            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

         

            app.UseAuthentication();
            app.UseMiddleware(typeof(ExceptionMiddleware));


            // global cors policy
            //app.UseCors(
            //    x => x
            //    .AllowAnyOrigin()
            //    .AllowAnyMethod()
            //    .AllowAnyHeader()
            //    .AllowCredentials()
            //    );

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin()
                .AllowAnyHeader().AllowAnyMethod().AllowCredentials();
            });


            app.UseSignalR(routes => {
                routes.MapHub<NotifyHub>("/notify");
            });

            app.UseSignalR(routes => {
                routes.MapHub<NotifyHub>("/notifyMain");
            });

            app.UseMvc();
        }
    }
}
